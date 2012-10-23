using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using PaysonIntegration.Utils;

namespace PaysonIntegration
{
    public class HttpCaller
    {
        private static string ModuleInfo
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string version = assembly.GetName().Version.ToString();
                return string.Format("payson_apidotnet|{0}|NONE", version);
            }
        }


        public IDictionary<string, string> MakeHttpPostRequest(string url, string userId, string password, string applicationId, int timeout, IDictionary<string, string> nvpCollection)
        {
            return NvpCodec.ConvertToNameValueCollection(MakeHttpPostRequest(url, userId, password, applicationId, timeout, NvpCodec.ConvertToNvpString(nvpCollection)));
        }

        public string MakeHttpPostRequest(string url, string userId, string password, string applicationId, int timeout, string nvpString)
        {
            HttpWebRequest objRequest = (HttpWebRequest)WebRequest.Create(url);
            objRequest.Timeout = timeout;
            objRequest.Method = "POST";
            objRequest.ContentLength = nvpString.Length;
            objRequest.Headers.Add("PAYSON-SECURITY-USERID", userId);
            objRequest.Headers.Add("PAYSON-SECURITY-PASSWORD", password);
            objRequest.Headers.Add("PAYSON-MODULE-INFO", ModuleInfo);
            if (!string.IsNullOrEmpty(applicationId))
            {
                objRequest.Headers.Add("PAYSON-APPLICATION-ID", applicationId);
            }
            using (StreamWriter contentWriter = new StreamWriter(objRequest.GetRequestStream()))
            {
                contentWriter.Write(nvpString);
            }

            //Catch Response
            HttpWebResponse objResponse = (HttpWebResponse)objRequest.GetResponse();
            string responseContent;
            using (StreamReader contentReader = new StreamReader(objResponse.GetResponseStream()))
            {
                responseContent = contentReader.ReadToEnd();
            }
            return responseContent;
        }

    }
}

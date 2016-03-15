using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using PaysonIntegration.Exceptions;
using PaysonIntegration.Response;
using PaysonIntegration.Utils;

namespace PaysonIntegration.Communication
{
    internal class PaysonClient : IPaysonClient
    {

        public PayResponse CreatePayment(string url, string userId, string userKey, string applicationId, int timeout, Data.PayData data)
        {
            return new PayResponse(Post(url, userId, userKey, applicationId, timeout, data.AsNvpDictionary()));
        }

        public PaymentUpdateResponse UpdatePayment(string url, string userId, string userKey, string applicationId, int timeout, Data.PaymentUpdateData data)
        {
            return new PaymentUpdateResponse(Post(url, userId, userKey, applicationId, timeout, data.AsNvpDictionary()));
        }

        public PaymentDetailsResponse CreatePaymentDetails(string url, string userId, string userKey, string applicationId, int timeout, Data.PaymentDetailsData data)
        {
            return new PaymentDetailsResponse(Post(url, userId, userKey, applicationId, timeout, data.AsNvpDictionary()));
        }

        public AccountDetailsResponse CreateAccountDetails(string url, string userId, string userKey, string applicationId, int timeout)
        {
            return new AccountDetailsResponse(Post(url, userId, userKey, applicationId, timeout, new Dictionary<string, string> {}));
        }

        public ValidateResponse ValidateIpnContent(string url, string userId, string userKey, string applicationId, int timeout, string content)
        {
            return new ValidateResponse(Post(url, userId, userKey, applicationId, timeout, content), content);
        }

        private IDictionary<string, string> Post(string url, string userId, string userKey, string applicationId, int timeout, IDictionary<string, string> nvpCollection)
        {
            return NvpCodec.ConvertToNameValueCollection(Post(url, userId, userKey, applicationId, timeout, NvpCodec.ConvertToNvpString(nvpCollection)));
        }

        private string Post(string url, string userId, string userKey, string applicationId, int timeout, string nvpString)
        {
            try
            {
                var objRequest = (HttpWebRequest)WebRequest.Create(url);
                objRequest.Timeout = timeout;
                objRequest.Method = "POST";
                objRequest.ContentLength = nvpString.Length;
                objRequest.Headers.Add("PAYSON-SECURITY-USERID", userId);
                objRequest.Headers.Add("PAYSON-SECURITY-PASSWORD", userKey);
                objRequest.Headers.Add("PAYSON-MODULE-INFO", ModuleInfo());
                if (!string.IsNullOrEmpty(applicationId))
                {
                    objRequest.Headers.Add("PAYSON-APPLICATION-ID", applicationId);
                }
                using (var contentWriter = new StreamWriter(objRequest.GetRequestStream()))
                {
                    contentWriter.Write(nvpString);
                }

                //Catch Response
                var objResponse = (HttpWebResponse)objRequest.GetResponse();
                string responseContent;
                using (var contentReader = new StreamReader(objResponse.GetResponseStream()))
                {
                    responseContent = contentReader.ReadToEnd();
                }
                return responseContent;                            
            } catch (Exception ex)
            {
                throw new PaysonException(ex);
            }
        }

        private static string ModuleInfo()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string version = assembly.GetName().Version.ToString();
            return string.Format("payson_apidotnet|{0}|NONE", version);
        }
    }
}

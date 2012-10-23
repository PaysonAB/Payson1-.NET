using System.Collections.Generic;
using System.Text;
using System.Web;

namespace PaysonIntegration.Utils
{
    public class NvpCodec
    {
        public static IDictionary<string, string> ConvertToNameValueCollection(string nvpString)
        {
            var nvpCollection = new Dictionary<string, string>();

            foreach (string nvp in nvpString.Split(new[] { '&' }))
            {
                string[] tokens = nvp.Split(new[] { '=' });
                if (tokens.Length >= 2)
                {
                    string name = HttpUtility.UrlDecode(tokens[0]);
                    string value = HttpUtility.UrlDecode(tokens[1]);
                    nvpCollection.Add(name, value);
                }
            }

            return nvpCollection;
        }

        public static string ConvertToNvpString(IDictionary<string, string> nvpCollection)
        {
            var sb = new StringBuilder();

            bool firstPair = true;
            foreach (string key in nvpCollection.Keys)
            {
                string name = HttpUtility.UrlEncode(key);
                string value = HttpUtility.UrlEncode(nvpCollection[key]);
                if (firstPair)
                {
                    sb.Append(string.Format("{0}={1}", name, value));
                    firstPair = false;
                }
                else
                {
                    sb.Append(string.Format("&{0}={1}", name, value));
                }
            }

            return sb.ToString();
        }
    }
}

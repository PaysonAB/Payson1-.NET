using System.Collections.Generic;
using System.Collections.Specialized;
using PaysonIntegration.Utils;

namespace PaysonIntegration.Response
{
    public abstract class Response
    {
        public IDictionary<string,string> NvpContent { get; protected set; }

        public bool Success { get; protected set; }
        public string Timestamp { get; protected set; }
        public string CorrelationId { get; set; }

        public  NameValueCollection ErrorMessages { get; protected set; }

        protected Response(IDictionary<string,string> nvpResponseContent)
        {
            NvpContent = nvpResponseContent;
            SetCommon(nvpResponseContent);
            InitiateResponse(nvpResponseContent);
        }

        private void SetCommon(IDictionary<string, string> nvpResponseContent)
        {
            Success = ((nvpResponseContent.GetValueOrNull("responseEnvelope.ack") ?? "").ToUpper() == "SUCCESS");

            ErrorMessages = GetErrorMessages(nvpResponseContent);

            Timestamp = nvpResponseContent.GetValueOrNull("responseEnvelope.timestamp") ?? "";

            CorrelationId = nvpResponseContent.GetValueOrNull("responseEnvelope.correlationId") ?? "";
        }

        private static NameValueCollection GetErrorMessages(IDictionary<string, string> nvpResponseContent)
        {
            var messages = new NameValueCollection();            
            int i = 0;
            while ( nvpResponseContent.ContainsKey(string.Format("errorList.error({0}).errorId", i)) )
            {
                var message = nvpResponseContent[string.Format("errorList.error({0}).message", i)];
                if (nvpResponseContent.ContainsKey(string.Format("errorList.error({0}).parameter", i)))
                    message += " (" + nvpResponseContent[string.Format("errorList.error({0}).parameter", i)] + ")";

                messages.Add(   nvpResponseContent[string.Format("errorList.error({0}).errorId", i)],
                                message);
                i++;
            }
            return messages;
        }

        protected abstract void InitiateResponse(IDictionary<string, string> nvpResponseContent);
    }
}
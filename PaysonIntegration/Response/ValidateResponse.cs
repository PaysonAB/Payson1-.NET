using PaysonIntegration.Utils;

namespace PaysonIntegration.Response
{
    public class ValidateResponse
    {
        public bool Success { get; private set; }
        public string Content { get; private set; }

        public string UnprocessedIpnMessage { get; private set; }
        public PaymentDetails ProcessedIpnMessage { get; private set; }

        public ValidateResponse(string responseContent, string validatedContent)
        {
            Content = responseContent;
            Success = (responseContent == "VERIFIED");
            UnprocessedIpnMessage = validatedContent;
            if (!string.IsNullOrWhiteSpace(validatedContent))
                ProcessedIpnMessage = new PaymentDetails(validatedContent);
            else
            {
                ProcessedIpnMessage = null;
                Success = false;
            }
        }
    }
}

using PaysonIntegration.Data;
using PaysonIntegration.Response;

namespace PaysonIntegration.Communication
{
    internal interface IPaysonClient
    {
        PayResponse CreatePayment(string url, string userId, string userKey, string applicationId, int timeout, PayData data);
        PaymentUpdateResponse UpdatePayment(string url, string userId, string userKey, string applicationId, int timeout, PaymentUpdateData data);
        PaymentDetailsResponse CreatePaymentDetails(string url, string userId, string userKey, string applicationId, int timeout, PaymentDetailsData data);
        ValidateResponse ValidateIpnContent(string url, string userId, string userKey, string applicationId, int timeout, string content);
    }
}

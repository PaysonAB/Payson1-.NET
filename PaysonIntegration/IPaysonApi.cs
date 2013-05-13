using PaysonIntegration.Data;
using PaysonIntegration.Response;

namespace PaysonIntegration
{
    public interface IPaysonApi
    {
        int Timeout { get; set; }
        bool IsTestMode { get; set; }
        string GetForwardPayUrl(string token);
        PayResponse MakePayRequest(PayData data);
        PaymentUpdateResponse MakePaymentUpdateRequest(PaymentUpdateData data);
        PaymentDetailsResponse MakePaymentDetailsRequest(PaymentDetailsData data);
        ValidateResponse MakeValidateIpnContentRequest(string content);
    }
}
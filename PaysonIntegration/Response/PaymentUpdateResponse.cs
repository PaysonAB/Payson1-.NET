using System.Collections.Generic;

namespace PaysonIntegration.Response
{
    public class PaymentUpdateResponse : Response
    {
        public PaymentUpdateResponse(IDictionary<string, string> nvpResponseContent) : base(nvpResponseContent)
        {
        }

        protected override void InitiateResponse(IDictionary<string, string> nvpResponseContent)
        {
        }
    }
}

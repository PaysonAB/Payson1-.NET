using System;
using System.Collections.Generic;
using PaysonIntegration.Utils;

namespace PaysonIntegration.Response
{
    public class PaymentDetailsResponse : Response
    {
        public PaymentDetails PaymentDetails { get; private set; }

        public PaymentDetailsResponse(IDictionary<string, string> nvpResponseContent)
            : base(nvpResponseContent)
        {
        }

        protected override void InitiateResponse(IDictionary<string, string> nvpResponseContent)
        {
            if(!Success)
            {
                return;
            }

            PaymentDetails = new PaymentDetails(nvpResponseContent);
        }

    }
}

using System.Collections.Generic;
using PaysonIntegration.Utils;

namespace PaysonIntegration.Response
{
    public class PayResponse : Response
    {
        public string Token { get; private set; }

        public PayResponse(IDictionary<string,string> nvpResponseContent) : base(nvpResponseContent)
        {
        }

        protected override void InitiateResponse(IDictionary<string, string> nvpResponseContent)
        {
            Token = nvpResponseContent.GetValueOrNull("TOKEN") ?? "";
        }
    }
}

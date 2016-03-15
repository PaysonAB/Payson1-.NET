using System.Collections.Generic;
using PaysonIntegration.Utils;

namespace PaysonIntegration.Response
{
    public class AccountDetailsResponse : Response
    {
        public AccountDetails AccountDetails { get; private set; }

        public AccountDetailsResponse(IDictionary<string, string> nvpResponseContent)
            : base(nvpResponseContent)
        {
        }

        protected override void InitiateResponse(IDictionary<string, string> nvpResponseContent)
        {
            if(!Success)
            {
                return;
            }

            AccountDetails = new AccountDetails(nvpResponseContent);
        }

    }
}

using System.Collections.Generic;
using System.Globalization;

namespace PaysonIntegration.Utils
{
    public class AccountDetails
    {
        public string AccountEmail { get; protected set; }
        public bool EnabledForInvoice{ get; protected set; }
        public bool EnabledForPaymentPlan { get; protected set; }
        public string AgentId { get; protected set; }

        public AccountDetails(IDictionary<string, string> nvpContent)
        {
            InitiateAccountDetails(nvpContent);
        }

        public AccountDetails(string nvpContent)
        {
            InitiateAccountDetails(nvpContent);
        }

        protected void InitiateAccountDetails(IDictionary<string, string> nvpContent)
        {

            AccountEmail = nvpContent.GetValueOrNull("accountEmail");
            AgentId = nvpContent.GetValueOrNull("merchantId");
            AccountEmail = nvpContent.GetValueOrNull("accountEmail");
            EnabledForInvoice = nvpContent.GetValueOrNull("enabledForInvoice") == "TRUE";
            EnabledForPaymentPlan = nvpContent.GetValueOrNull("enabledForPaymentPlan") == "TRUE";
        }

        protected void InitiateAccountDetails(string nvpContent)
        {
            InitiateAccountDetails(NvpCodec.ConvertToNameValueCollection(nvpContent));
        }

    }
}
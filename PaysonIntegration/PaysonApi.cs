using System;
using System.Configuration;
using PaysonIntegration.Communication;
using PaysonIntegration.Data;
using PaysonIntegration.Response;

namespace PaysonIntegration
{
    public class PaysonApi : IPaysonApi
    {
        private static readonly string ApiHost = ConfigurationManager.AppSettings["Payson.ApiHost"] ?? @"https://api.payson.se/";
        private static readonly string ForwardHost = ConfigurationManager.AppSettings["Payson.ForwardHost"] ?? @"https://www.payson.se/paySecure/?token=";

        private static readonly string ApiHostTest = ConfigurationManager.AppSettings["Payson.ApiHostTest"] ?? @"http://test-api.payson.se/";
        private static readonly string ForwardHostTest = ConfigurationManager.AppSettings["Payson.ForwardHostTest"] ?? @"http://test-www.payson.se/paySecure/?token=";

        private string _payUrl;
        private string _paymentDetailsUrl;
        private string _paymentUpdateUrl;
        private string _validateUrl;
        private string _payForwardUrlWithoutToken;

        private readonly string _userId;
        private readonly string _userKey;
        private readonly string _applicationId;
        private IPaysonClient _client;

        private bool _isTestMode;
        
        public int Timeout { get; set; }
        
        public bool IsTestMode
        {
            get { return _isTestMode; }
            set
            {
                _isTestMode = value;
                SetUrls();
            }
        }
        
        public PaysonApi(string userId, string userKey, string applicationId = null, bool isTestMode = false, int timeout = 50000)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("UserId cannot be null or empty");
            if (string.IsNullOrEmpty(userKey))
                throw new ArgumentException("UserKey cannot be null or empty");

            _userId = userId;
            _userKey = userKey;
            _applicationId = applicationId;
            Timeout = timeout;
            IsTestMode = isTestMode;
            SetUrls();
            InitPaysonClient();
        }

        private void SetUrls()
        {
            var host = IsTestMode ? ApiHostTest : ApiHost;
            _payUrl = host + Constants.ApiVersion + @"/Pay/";
            _paymentDetailsUrl = host + Constants.ApiVersion + @"/PaymentDetails/";
            _paymentUpdateUrl = host + Constants.ApiVersion + @"/PaymentUpdate/";
            _validateUrl = host + Constants.ApiVersion + @"/Validate/";

            var forwardHost = _isTestMode ? ForwardHostTest : ForwardHost;
            _payForwardUrlWithoutToken = forwardHost;
        }

        private void InitPaysonClient()
        {
            _client = new PaysonClient();
        }

        public string GetForwardPayUrl(string token)
        {
            return _payForwardUrlWithoutToken + token;
        }

        public PayResponse MakePayRequest(PayData data)
        {
            return _client.CreatePayment(_payUrl, _userId, _userKey, _applicationId, Timeout, data);
        }

        public PaymentUpdateResponse MakePaymentUpdateRequest(PaymentUpdateData data)
        {
            return _client.UpdatePayment(_paymentUpdateUrl, _userId, _userKey, _applicationId, Timeout, data);
        }

        public PaymentDetailsResponse MakePaymentDetailsRequest(PaymentDetailsData data)
        {
            return _client.CreatePaymentDetails(_paymentDetailsUrl, _userId, _userKey, _applicationId, Timeout, data);
        }

        public ValidateResponse MakeValidateIpnContentRequest(string content)
        {
            return _client.ValidateIpnContent(_validateUrl, _userId, _userKey, _applicationId, Timeout, content);
        }

        #region Payson Internal
        /// <summary>
        /// This method is only for internal Payson use. Never use this as it will not work outside Payson test environment
        /// </summary>
        public void UseStage()
        {
            _payUrl = "https://mvcapi.payson.stage/1.0/Pay/";
            _payForwardUrlWithoutToken = "https://app.payson.stage/paySecure/?token=";
        }
        #endregion
    }
}

using System;
using System.Configuration;
using PaysonIntegration.Data;
using PaysonIntegration.Response;

namespace PaysonIntegration
{
    public class PaysonApi
    {
        private const string ApiVersion = @"1.0";

        private static readonly string ApiHost = ConfigurationManager.AppSettings["Payson.ApiHost"] ?? @"https://api.payson.se/";
        private static readonly string ForwardHost = ConfigurationManager.AppSettings["Payson.ForwardHost"] ?? @"https://www.payson.se/paySecure/?token=";

        private static readonly string ApiHostTest = ConfigurationManager.AppSettings["Payson.ApiHostTest"] ?? @"http://test-api.payson.se/";
        private static readonly string ForwardHostTest = ConfigurationManager.AppSettings["Payson.ForwardHostTest"] ?? @"http://test-www.payson.se/paySecure/?token=";

        private string payUrl;
        private string paymentDetailsUrl;
        private string paymentUpdateUrl;
        private string validateUrl;

        private string payForwardUrlWithoutToken;

        private int _timeout = 50000;
        public int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
            }
        }

        private bool isTestMode;
        public bool IsTestMode
        {
            get { return isTestMode; }
            set
            {
                isTestMode = value;
                SetUrls();
            }
        }



        private string UserId { get; set; }
        private string UserKey { get; set; }
        private string ApplicationId { get; set; }
        private HttpCaller HttpCaller { get; set; }

        public PaysonApi(string userId, string userKey, string applicationId = null, bool isTestMode = false)
        {
            IsTestMode = isTestMode;

            SetUserId(userId);
            SetUserKey(userKey);
            SetApplicationId(applicationId);
            SetUrls();

            HttpCaller = new HttpCaller();
        }

        private void SetUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("UserId cannot be null or empty");

            UserId = userId;
        }

        private void SetUserKey(string userKey)
        {
            if (string.IsNullOrEmpty(userKey))
                throw new ArgumentException("UserKey cannot be null or empty");

            UserKey = userKey;
        }

        private void SetApplicationId(string applicationId)
        {
            ApplicationId = applicationId;
        }

        private void SetUrls()
        {
            var host = isTestMode ? ApiHostTest : ApiHost;
            payUrl = host + ApiVersion + @"/Pay/";
            paymentDetailsUrl = host + ApiVersion + @"/PaymentDetails/";
            paymentUpdateUrl = host + ApiVersion + @"/PaymentUpdate/";
            validateUrl = host + ApiVersion + @"/Validate/";

            var forwardHost = isTestMode ? ForwardHostTest : ForwardHost;
            payForwardUrlWithoutToken = forwardHost;
        }

        /// <summary>
        /// This method is only for internal Payson use. Never use this as it will not work outside Payson test environment
        /// </summary>
        public void UseStage()
        {
            payUrl = "https://mvcapi.payson.stage/1.0/Pay/";
            payForwardUrlWithoutToken = "https://app.payson.stage/paySecure/?token=";
        }

        public string GetForwardPayUrl(string token)
        {
            return payForwardUrlWithoutToken + token;
        }

        public PayResponse MakePayRequest(PayData data)
        {
            return new PayResponse(HttpCaller.MakeHttpPostRequest(payUrl, UserId, UserKey, ApplicationId, _timeout, data.AsNvpDictionary()));
        }

        public PaymentUpdateResponse MakePaymentUpdateRequest(PaymentUpdateData data)
        {
            return new PaymentUpdateResponse(HttpCaller.MakeHttpPostRequest(paymentUpdateUrl, UserId, UserKey, ApplicationId, _timeout, data.AsNvpDictionary()));
        }

        public PaymentDetailsResponse MakePaymentDetailsRequest(PaymentDetailsData data)
        {
            return new PaymentDetailsResponse(HttpCaller.MakeHttpPostRequest(paymentDetailsUrl, UserId, UserKey, ApplicationId, _timeout, data.AsNvpDictionary()));
        }

        public ValidateResponse MakeValidateIpnContentRequest(string content)
        {
            return new ValidateResponse(HttpCaller.MakeHttpPostRequest(validateUrl, UserId, UserKey, ApplicationId, _timeout, content), content);
        }
    }
}

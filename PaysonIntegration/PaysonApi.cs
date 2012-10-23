using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PaysonIntegration.Data;
using PaysonIntegration.Response;

namespace PaysonIntegration
{

    public class PaysonApi
    {
        //Preferably these constants would be put in a config file
        public const string Host = @"https://api.payson.se/";
        public const string ApiVersion = @"1.0";
        public const string PayUrl = Host + ApiVersion + @"/Pay/";
        public const string PaymentDetailsUrl = Host + ApiVersion + @"/PaymentDetails/";
        public const string PaymentUpdateUrl = Host + ApiVersion + @"/PaymentUpdate/";
        public const string ValidateUrl = Host + ApiVersion + @"/Validate/";
        
        private const string PayForwardUrlWithoutToken = @"https://www.payson.se/paySecure/?token=";

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

        private string UserId { get; set; }
        private string UserKey { get; set; }
        private string ApplicationId { get; set; }
        private HttpCaller HttpCaller { get; set; }

        public PaysonApi(string userId, string userKey, string applicationId = null)
        {
            SetUserId(userId);
            SetUserKey(userKey);
            SetApplicationId(applicationId);
            
            HttpCaller = new HttpCaller();
        }

        private void SetUserId(string userId)
        {
            if(string.IsNullOrEmpty(userId))
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

        public string GetForwardPayUrl(string token)
        {
            return PayForwardUrlWithoutToken + token;
        }

        public PayResponse MakePayRequest(PayData data)
        {
            return new PayResponse( HttpCaller.MakeHttpPostRequest(PayUrl, UserId, UserKey, ApplicationId, _timeout, data.AsNvpDictionary()) );
        }

        public PaymentUpdateResponse MakePaymentUpdateRequest(PaymentUpdateData data)
        {
            return new PaymentUpdateResponse( HttpCaller.MakeHttpPostRequest(PaymentUpdateUrl, UserId, UserKey, ApplicationId, _timeout, data.AsNvpDictionary()) );
        }

        public PaymentDetailsResponse MakePaymentDetailsRequest(PaymentDetailsData data)
        {
            return new PaymentDetailsResponse(HttpCaller.MakeHttpPostRequest(PaymentDetailsUrl, UserId, UserKey, ApplicationId, _timeout, data.AsNvpDictionary()));
        }

        public ValidateResponse MakeValidateIpnContentRequest(string content)
        {
            return new ValidateResponse(HttpCaller.MakeHttpPostRequest(ValidateUrl, UserId, UserKey, ApplicationId, _timeout, content), content);
        }
    }
}

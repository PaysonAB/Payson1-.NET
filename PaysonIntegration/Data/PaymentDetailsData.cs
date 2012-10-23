using System;
using System.Collections.Generic;

namespace PaysonIntegration.Data
{
    public class PaymentDetailsData
    {
        public string Token { get; private set; }

        public PaymentDetailsData(string token)
        {
            if(string.IsNullOrEmpty(token))
                throw new ArgumentException("token cannot be null or empty");

            Guid parsedToken;
            if(!Guid.TryParse(token, out parsedToken))
                throw new ArgumentException("token was not recognized as a valid Guid");

            Token = token;
            
        }

        public IDictionary<string, string> AsNvpDictionary()
        {
            return new Dictionary<string, string>
                       {
                           {"token", Token}
                       };
        }
    }
}

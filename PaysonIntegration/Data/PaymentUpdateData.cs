using System;
using System.Collections.Generic;
using PaysonIntegration.Utils;

namespace PaysonIntegration.Data
{
    public class PaymentUpdateData
    {
        public string Token { get; private set; }
        public PaymentUpdateAction Action { get; private set; }



        public PaymentUpdateData(string token, PaymentUpdateAction action)
        {
            SetToken(token);
            Action = action;
        }

        private void SetToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("token cannot be null or empty");
            Guid parsedToken;
            if(!Guid.TryParse(token, out parsedToken))
                throw new ArgumentException("token is not a valid Guid");
            if(parsedToken == Guid.Empty)
                throw new ArgumentException("token cannot be an empty Guid");

            Token = token;
        }

        public IDictionary<string, string> AsNvpDictionary()
        {
            return new Dictionary<string, string>
                        {
                            {"token", Token},
                            {"action", Action.ToString().ToUpper()}
                        };
        }
    }
}
using System;

namespace PaysonIntegration.Exceptions
{
    public class PaysonException : Exception
    {
        private new const string Message = "An exception occured while communicating with the Payson API. See inner exception for more details.";

        public PaysonException(Exception innerException) : base(Message, innerException){}

        public PaysonException(string message, Exception innerException) : base(message, innerException){}

    }
}

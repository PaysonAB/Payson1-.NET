using System;

namespace PaysonIntegration.Utils
{

    //TODO: Amount cannot be larger than $10000 USD..

    public class Receiver : User
    {
        public bool? IsPrimaryReceiver { get; private set; }
        public decimal Amount { get; private set; }

        public Receiver(string email, decimal amount) : base(email)
        {            
            SetAmount(amount);
        }

        private void SetAmount(decimal amount)
        {
            if (amount <= 0.0m)
                throw new ArgumentException("amount must be a positive value");

            Amount = amount;
        }

        public void SetPrimaryReceiver(bool b)
        {
            IsPrimaryReceiver = b;
        }
    }
}
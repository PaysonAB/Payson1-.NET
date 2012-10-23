using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PaysonIntegration.Utils
{
    public class User
    {
        public string Email { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        public User(string email)
        {
            SetEmail(email);
        }

        private void SetEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentException("email cannot be null or empty");
            if (email.Length > Settings.MaxEmailLength)
                throw new ArgumentException(string.Format("Email can be at most {0} characters long", Settings.MaxEmailLength));

            Email = email;
        }

        public void SetFirstName(string firstName)
        {
            if (string.IsNullOrEmpty(firstName))
                throw new ArgumentException("firstName cannot be null or empty");

            if (firstName.Length > Settings.MaxNameLength)
                throw new ArgumentException(string.Format("First name can be at most {0} characters long", Settings.MaxNameLength));

            FirstName = firstName;
        }

        public void SetLastName(string lastName)
        {
            if (string.IsNullOrEmpty(lastName))
                throw new ArgumentException("lastName cannot be null or empty");

            if (lastName.Length > Settings.MaxNameLength)
                throw new ArgumentException(string.Format("Last name can be at most {0} characters long", Settings.MaxNameLength));

            LastName = lastName;
        }

    }
}

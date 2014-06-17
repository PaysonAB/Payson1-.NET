using System;

namespace PaysonIntegration.Utils
{
    public class User
    {
        private string _email;
        private string _firstName;
        private string _lastName;

        public string Email { get { return _email; } set { SetEmail(value); } }
        public string FirstName { get { return _firstName; } set { SetFirstName(value); } }
        public string LastName { get { return _lastName; } set { SetLastName(value); } }

        public User(string email) : this(email, string.Empty, string.Empty) { }

        public User(string email, string firstName, string lastName)
        {
            SetEmail(email);
            SetFirstName(firstName);
            SetLastName(lastName);
        }

        private void SetEmail(string email)
        {
            if (email.Length > Settings.MaxEmailLength)
                throw new ArgumentException(string.Format("Email can be at most {0} characters long", Settings.MaxEmailLength));

            _email = email;
        }

        private void SetFirstName(string firstName)
        {
            if (firstName.Length > Settings.MaxNameLength)
                throw new ArgumentException(string.Format("First name can be at most {0} characters long", Settings.MaxNameLength));

            _firstName = firstName;
        }

        private void SetLastName(string lastName)
        {
            if (lastName.Length > Settings.MaxNameLength)
                throw new ArgumentException(string.Format("Last name can be at most {0} characters long", Settings.MaxNameLength));

            _lastName = lastName;
        }

    }
}

namespace LM_Exchange.Custom_Exception
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
    public class InvalidName : Exception
    {
        public InvalidName(string message) : base(message)
        {
        }
    }

    public class InsufficientbalanceException : Exception
    {
        public InsufficientbalanceException(string message) : base(message)
        {
        }
    }
    public class SelfTransationException : Exception
    {
        public SelfTransationException(string message) : base(message)
        {
        }
    }
    public class InvalidAmountExpection : Exception
    {
        public InvalidAmountExpection(string message) : base(message)
        {
        }
    }
    public class InvalidEmailOrPassword : Exception
    {
        public InvalidEmailOrPassword(string message) : base(message)
        {
        }
    }
    public class EmailAlreadyRegistred : Exception
    {
        public EmailAlreadyRegistred(string message) : base(message)
        {
        }
    }
    public class InvalidRole : Exception
    {
        public InvalidRole(string message) : base(message)
        {
        }
    }
    public class InvalidPassword : Exception
    {
        public InvalidPassword(string message) : base(message)
        {
        }
    }
    public class InvalidEmail : Exception
    {
        public InvalidEmail(string message) : base(message)
        {

        }
    }
    public class InvalidAadar : Exception
    {
        public InvalidAadar(string message) : base(message)
        {

        }
    }

    public class InvalidMobile : Exception
    {
        public InvalidMobile(string message) : base(message)
        {

        }
    }

    public class InvalidId : Exception
    {
        public InvalidId(string message) : base(message)
        {

        }
    }
}

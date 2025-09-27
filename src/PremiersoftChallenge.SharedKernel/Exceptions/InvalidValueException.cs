namespace PremiersoftChallenge.SharedKernel.Exceptions
{
    public class InvalidValueException : Exception
    {
        protected InvalidValueException() { }

        public InvalidValueException(string? message) : base(message) { }
    }
}

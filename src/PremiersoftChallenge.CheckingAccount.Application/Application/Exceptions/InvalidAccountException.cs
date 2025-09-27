namespace Application.Exceptions
{
    internal class InvalidAccountException : Exception
    {
        protected InvalidAccountException() { }

        public InvalidAccountException(string? message) : base(message) { }
    }
}

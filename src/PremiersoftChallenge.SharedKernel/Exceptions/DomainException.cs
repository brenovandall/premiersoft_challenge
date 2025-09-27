namespace PremiersoftChallenge.SharedKernel.Exceptions
{
    public class DomainException : Exception
    {
        protected DomainException() { }

        public DomainException(string? message) : base("Domain exception: " + message) { }
    }
}

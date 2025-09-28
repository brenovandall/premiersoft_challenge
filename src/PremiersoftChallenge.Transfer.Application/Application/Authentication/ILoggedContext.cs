namespace Application.Authentication
{
    public interface ILoggedContext
    {
        Guid Id { get; }
        string? Token { get; }
    }
}

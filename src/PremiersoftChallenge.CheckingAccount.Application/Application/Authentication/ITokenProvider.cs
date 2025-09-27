namespace Application.Authentication
{
    public interface ITokenProvider
    {
        string Create(Domain.CheckingAccount checkingAccount);
    }
}

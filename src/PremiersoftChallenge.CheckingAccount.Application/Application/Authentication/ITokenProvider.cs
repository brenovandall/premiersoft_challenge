using Domain;

namespace Application.Authentication
{
    public interface ITokenProvider
    {
        string Create(ICheckingAccount checkingAccount);
    }
}

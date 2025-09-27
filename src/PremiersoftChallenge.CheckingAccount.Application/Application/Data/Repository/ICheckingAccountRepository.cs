using Domain;

namespace Application.Data.Repository
{
    public interface ICheckingAccountRepository
    {
        void Add(ICheckingAccount checkingAccount);
        ICheckingAccount? GetByAccountNumberOrName(string searchString);
    }
}

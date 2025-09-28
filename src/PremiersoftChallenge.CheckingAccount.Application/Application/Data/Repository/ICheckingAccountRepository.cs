using Domain;

namespace Application.Data.Repository
{
    public interface ICheckingAccountRepository
    {
        void Add(ICheckingAccount checkingAccount);
        long Count();
        ICheckingAccount? GetById(Guid id);
        ICheckingAccount? GetByAccountNumberOrName(string searchString);
        void Update(ICheckingAccount checkingAccount);
    }
}

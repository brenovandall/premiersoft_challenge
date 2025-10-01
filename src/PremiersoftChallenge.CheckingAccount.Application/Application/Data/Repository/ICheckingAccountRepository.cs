using Domain;

namespace Application.Data.Repository
{
    public interface ICheckingAccountRepository
    {
        Task Add(ICheckingAccount checkingAccount);
        Task<long> MaxAccountNumber();
        Task<ICheckingAccount?> GetById(Guid id);
        Task<ICheckingAccount?> GetByAccountNumberOrName(string searchString);
        Task Update(ICheckingAccount checkingAccount);
    }
}

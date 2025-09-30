using Domain;

namespace Application.Data.Repository
{
    public interface ITransactionRepository
    {
        Task Add(ITransaction transaction);
    }
}

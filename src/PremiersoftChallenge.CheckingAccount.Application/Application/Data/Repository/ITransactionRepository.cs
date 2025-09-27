using Domain;

namespace Application.Data.Repository
{
    public interface ITransactionRepository
    {
        void Add(ITransaction transaction);
    }
}

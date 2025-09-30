using Application.Transaction.Queries.GetTransactionById;

namespace Application.Services
{
    public interface ITransactionService
    {
        Task<GetTransactionByIdResponse?> GetById(Guid id);
    }
}

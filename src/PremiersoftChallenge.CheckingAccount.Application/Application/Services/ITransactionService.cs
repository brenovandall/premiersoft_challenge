using Application.Transaction.Queries.GetTransactionById;

namespace Application.Services
{
    public interface ITransactionService
    {
        GetTransactionByIdResponse? GetById(Guid id);
    }
}

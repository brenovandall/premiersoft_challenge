using Application.Dto;
using PremiersoftChallenge.BuildingBlocks.Results;

namespace Application.Services
{
    public interface ITransferService
    {
        Task<Result<Guid>> GetCheckingAccountIdByNumber(long accountNumber);
        Task<Result> SendTransactionRequest(string requestId, long? accountNumber, double value, string transactionFlow);
        Task<Result<TransactionDto>> GetTransactionById(string transactionId);
    }
}

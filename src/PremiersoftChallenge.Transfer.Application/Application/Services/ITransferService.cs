using PremiersoftChallenge.BuildingBlocks.Results;

namespace Application.Services
{
    public interface ITransferService
    {
        Task<Result<Guid>> GetCheckingAccountIdByNumber(long accountNumber);
        Task<Result> SendTransactionRequest(long? accountNumber, double value, string transactionFlow);
    }
}

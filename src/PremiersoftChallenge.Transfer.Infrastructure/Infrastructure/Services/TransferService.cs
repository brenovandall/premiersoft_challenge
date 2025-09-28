using Application.Dto;
using Application.Services;
using Infrastructure.Http;
using Microsoft.Extensions.Configuration;
using PremiersoftChallenge.BuildingBlocks.Results;

namespace Infrastructure.Services
{
    internal sealed class TransferService : ITransferService
    {
        private readonly IConfiguration _configuration;
        private readonly IApiCall _apiCall;

        public TransferService(IConfiguration configuration, IApiCall apiCall)
        {
            _configuration = configuration;
            _apiCall = apiCall;
        }

        private sealed record GetCheckingAccountIdByNumberResponse(string AccountId);

        public async Task<Result<Guid>> GetCheckingAccountIdByNumber(long accountNumber)
        {
            var baseUri = _configuration["ChekingAccountApi:BaseUri"];
            var result = await _apiCall.GetAsync<GetCheckingAccountIdByNumberResponse>($"{baseUri}/v1/checkingAccount/{accountNumber}");
            if (result.IsFailure)
            {
                return Result.Failure<Guid>(result.Error);
            }

            var valid = Guid.TryParse(result.Value.AccountId, out var parsedId);
            if (!valid)
            {
                return Result.Failure<Guid>(Error.Failure("INVALID_OPERATION", $"Não foi possível converter o valor {parsedId} para o formato esperado."));
            }

            return Result.Success(parsedId);
        }

        private sealed record TransactionRequest(string RequestId, long? AccountNumber, double Value, string TransactionFlow);

        public async Task<Result> SendTransactionRequest(string requestId, long? accountNumber, double value, string transactionFlow)
        {
            var baseUri = _configuration["ChekingAccountApi:BaseUri"];
            var request = new TransactionRequest(requestId, accountNumber, value, transactionFlow);
            var result = await _apiCall.PostAsync($"{baseUri}/v1/transaction/perform", request);
            if (result.IsFailure)
            {
                return Result.Failure(result.Error);
            }

            return Result.Success(result);
        }

        public sealed record GetTransactionByIdResult(TransactionDto Transaction);

        public async Task<Result<TransactionDto>> GetTransactionById(string transactionId)
        {
            var baseUri = _configuration["ChekingAccountApi:BaseUri"];
            var result = await _apiCall.GetAsync<GetTransactionByIdResult>($"{baseUri}/v1/transaction/{transactionId}");
            if (result.IsFailure)
            {
                return Result.Failure<TransactionDto>(result.Error);
            }

            return Result.Success(result.Value.Transaction);
        }
    }
}

using Application.Authentication;
using Application.Data.Repository;
using Application.Services;
using PremiersoftChallenge.BuildingBlocks.CQRS;
using PremiersoftChallenge.BuildingBlocks.Errors;
using PremiersoftChallenge.BuildingBlocks.Results;

namespace Application.Transfer.Commands.MakeTransfer
{
    public class MakeTransferHandler : ICommandHandler<MakeTransferCommand, bool>
    {
        private readonly ITransferRepository _transferRepository;
        private readonly ITransferService _transferService;
        private readonly ILoggedContext _loggedContext;

        public MakeTransferHandler(
            ITransferRepository transferRepository,
            ITransferService transferService,
            ILoggedContext loggedContext)
        {
            _transferRepository = transferRepository;
            _transferService = transferService;
            _loggedContext = loggedContext;
        }

        private const string Debit = "D";
        private const string Credit = "C";

        public async Task<Result<bool>> Handle(MakeTransferCommand command, CancellationToken cancellationToken)
        {
            var sourceTransactionId = Guid.NewGuid().ToString();
            var targetTransactionId = Guid.NewGuid().ToString();
            var normalizedValue = Math.Round(command.Value, 2);

            try
            {
                var targetId = await _transferService.GetCheckingAccountIdByNumber(command.TargetAccountNumber);
                if (targetId.IsFailure) return Result.Failure<bool>(targetId.Error);

                var sourceResult = await _transferService.SendTransactionRequest(sourceTransactionId, null, normalizedValue, Debit);
                if (sourceResult.IsFailure)
                    return await Fallback(sourceTransactionId, targetTransactionId, command.TargetAccountNumber, normalizedValue);

                var targetResult = await _transferService.SendTransactionRequest(targetTransactionId, command.TargetAccountNumber, normalizedValue, Credit);
                if (targetResult.IsFailure)
                    return await Fallback(sourceTransactionId, targetTransactionId, command.TargetAccountNumber, normalizedValue);

                var transfer = Domain.Transfer.Create(_loggedContext.Id.ToString(), targetId.Value.ToString(), normalizedValue);
                await _transferRepository.Add(transfer);

                return Result.Success(true);
            }
            catch (Exception)
            {
                return await Fallback(sourceTransactionId, targetTransactionId, command.TargetAccountNumber, normalizedValue);
            }
        }

        private async Task<Result<bool>> Fallback(string sourceTransactionId, string targetTransactionId, long targetAccountNumber, double value)
        {
            try
            {
                var sourceFallback = await HandleFallbackAsync(sourceTransactionId, null, value, Credit);

                if (sourceFallback.IsSuccess)
                {
                    var targetFallback = await HandleFallbackAsync(targetTransactionId, targetAccountNumber, value, Debit);
                    if (targetFallback.IsFailure) return Result.Failure<bool>(targetFallback.Error);
                }
                else
                {
                    return Result.Failure<bool>(sourceFallback.Error);
                }

                return Result.Failure<bool>(Error.Problem("SERVER_ERROR", "Um erro inesperado aconteceu ao atender a requisição." +
                                                                          "Os envolvidos foram reembolsados."));
            }
            catch (Exception)
            {
                return Result.Failure<bool>(Error.Problem("SERVER_ERROR", "Um erro inesperado aconteceu ao realizar um reembolso."));
            }
        }

        private async Task<Result> HandleFallbackAsync(string transactionId, long? accountNumber, double value, string flow)
        {
            var target = await _transferService.GetTransactionById(transactionId);
            if (!target.IsFailure)
            {
                var refund = await EnsureRefunded(transactionId, null, value, flow);
                if (refund.IsFailure) return Result.Failure<bool>(TransactionErrors.RefundFailed);
            }
            return Result.Success();
        }

        private async Task<Result> EnsureRefunded(string transactionId, long? accountNumber, double value, string flow)
        {
            var targetResult = await _transferService.SendTransactionRequest(Guid.NewGuid().ToString(), accountNumber, value, flow);
            if (targetResult.IsFailure) return Result.Failure<bool>(targetResult.Error);

            return Result.Success();
        }
    }
}

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

        public async Task<Result<bool>> Handle(MakeTransferCommand command, CancellationToken cancellationToken)
        {
            var sourceTransactionId = Guid.NewGuid().ToString();
            var targetTransactionId = Guid.NewGuid().ToString();

            try
            {
                var targetId = await _transferService.GetCheckingAccountIdByNumber(command.TargetAccountNumber);
                if (targetId.IsFailure)
                {
                    return Result.Failure<bool>(targetId.Error);
                }

                var sourceResult = await _transferService.SendTransactionRequest(sourceTransactionId, null, command.Value, "D");
                if (sourceResult.IsFailure)
                {
                    return Result.Failure<bool>(sourceResult.Error);
                }

                var targetResult = await _transferService.SendTransactionRequest(targetTransactionId, command.TargetAccountNumber, command.Value, "C");
                if (targetResult.IsFailure)
                {
                    return Result.Failure<bool>(targetResult.Error);
                }

                var transfer = Domain.Transfer.Create(_loggedContext.Id.ToString(), targetId.Value.ToString(), command.Value);
                _transferRepository.Add(transfer);

                return Result.Success(true);
            }
            catch (Exception ex)
            {
                var sourceFallback = await HandleFallbackAsync(sourceTransactionId, null, command.Value, "C");
                if (sourceFallback.IsSuccess)
                {
                    var targetFallback = await HandleFallbackAsync(targetTransactionId, command.TargetAccountNumber, command.Value, "D");
                    if (targetFallback.IsFailure)
                    {
                        return Result.Failure<bool>(targetFallback.Error);
                    }
                }
                else
                {
                    return Result.Failure<bool>(sourceFallback.Error);
                }

                return Result.Failure<bool>(Error.Failure("SERVER_ERROR", ex.Message));
            }
        }

        private async Task<Result> HandleFallbackAsync(string transactionId, long? accountNumber, double value, string flow)
        {
            var target = await _transferService.GetTransactionById(transactionId);
            if (!target.IsFailure)
            {
                var refund = await EnsureRefunded(transactionId, null, value, "C");
                if (refund.IsFailure)
                    return Result.Failure<bool>(TransactionErrors.RefundFailed);
            }
            return Result.Success();
        }

        private async Task<Result> EnsureRefunded(string transactionId, long? accountNumber, double value, string flow)
        {
            var targetResult = await _transferService.SendTransactionRequest(transactionId, accountNumber, value, flow);
            if (targetResult.IsFailure)
            {
                return Result.Failure<bool>(targetResult.Error);
            }

            return Result.Success();
        }
    }
}

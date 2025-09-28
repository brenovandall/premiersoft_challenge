using Application.Authentication;
using Application.Data.Repository;
using Application.Services;
using PremiersoftChallenge.BuildingBlocks.CQRS;
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
            var targetId = await _transferService.GetCheckingAccountIdByNumber(command.TargetAccountNumber);
            if (targetId.IsFailure)
            {
                return Result.Failure<bool>(targetId.Error);
            }

            var targetResult = await _transferService.SendTransactionRequest(command.TargetAccountNumber, command.Value, "C");
            if (targetResult.IsFailure)
            {
                return Result.Failure<bool>(targetResult.Error);
            }

            var sourceResult = await _transferService.SendTransactionRequest(null, command.Value, "D");
            if (sourceResult.IsFailure)
            {
                // todo: fallback
                return Result.Failure<bool>(sourceResult.Error);
            }

            var transfer = Domain.Transfer.Create(_loggedContext.Id.ToString(), targetId.Value.ToString(), command.Value);
            _transferRepository.Add(transfer);

            return Result.Success(true);
        }
    }
}

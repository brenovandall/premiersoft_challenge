using PremiersoftChallenge.BuildingBlocks.CQRS;

namespace Application.Transfer.Commands.MakeTransfer
{
    public sealed record MakeTransferCommand(long TargetAccountNumber, double Value) : ICommand<bool>;
}

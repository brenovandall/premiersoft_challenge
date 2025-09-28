using PremiersoftChallenge.BuildingBlocks.CQRS;

namespace Application.Transaction.Commands.MakeTransaction
{
    public sealed record MakeTransactionCommand(string RequestId, long? AccountNumber, double Value, string TransactionFlow)
        : ICommand<bool>;
}

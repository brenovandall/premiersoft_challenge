using PremiersoftChallenge.BuildingBlocks.CQRS;

namespace Application.Transaction.Commands.MakeTransaction
{
    public sealed record MakeTransactionCommand(long? AccountNumber, double Value, string TransactionFlow)
        : ICommand<bool>;
}

using Domain;
using PremiersoftChallenge.BuildingBlocks.CQRS;

namespace Application.Transaction.Queries.GetTransactionById
{
    public sealed record GetTransactionByIdQuery(string TransactionId) : IQuery<GetTransactionByIdResult>;
    public sealed record GetTransactionByIdResult(GetTransactionByIdResponse Transaction);
}

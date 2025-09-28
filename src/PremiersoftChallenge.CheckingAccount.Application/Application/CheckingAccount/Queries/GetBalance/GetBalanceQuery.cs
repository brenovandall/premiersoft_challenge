using PremiersoftChallenge.BuildingBlocks.CQRS;

namespace Application.CheckingAccount.Queries.GetBalance
{
    public sealed record GetBalanceQuery(Guid AccountId) : IQuery<GetBalanceResponse>;
}

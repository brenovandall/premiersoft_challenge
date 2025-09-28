using PremiersoftChallenge.BuildingBlocks.CQRS;

namespace Application.CheckingAccount.Queries.GetCheckingAccountIdByNumber
{
    public sealed record GetCheckingAccountIdByNumberQuery(long AccountNumber) : IQuery<GetCheckingAccountIdByNumberResult>;
    public sealed record GetCheckingAccountIdByNumberResult(string AccountId);
}

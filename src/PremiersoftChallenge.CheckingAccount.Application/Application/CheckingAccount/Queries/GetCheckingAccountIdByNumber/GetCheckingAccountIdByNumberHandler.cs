using Application.Data.Repository;
using PremiersoftChallenge.BuildingBlocks.CQRS;
using PremiersoftChallenge.BuildingBlocks.Errors;
using PremiersoftChallenge.BuildingBlocks.Results;

namespace Application.CheckingAccount.Queries.GetCheckingAccountIdByNumber
{
    public class GetCheckingAccountIdByNumberHandler : IQueryHandler<GetCheckingAccountIdByNumberQuery, GetCheckingAccountIdByNumberResult>
    {
        private readonly ICheckingAccountRepository _repository;

        public GetCheckingAccountIdByNumberHandler(ICheckingAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<GetCheckingAccountIdByNumberResult>> Handle(GetCheckingAccountIdByNumberQuery query, CancellationToken cancellationToken)
        {
            var account = await _repository.GetByAccountNumberOrName(query.AccountNumber.ToString());
            if (account == null)
            {
                return Result.Failure<GetCheckingAccountIdByNumberResult>(CheckingAccountErrors.InvalidAccount);
            }

            return Result.Success(new GetCheckingAccountIdByNumberResult(account.Id.ToString()));
        }
    }
}

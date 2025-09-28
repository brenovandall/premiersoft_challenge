using Application.Services;
using PremiersoftChallenge.BuildingBlocks.CQRS;
using PremiersoftChallenge.BuildingBlocks.Errors;
using PremiersoftChallenge.BuildingBlocks.Results;

namespace Application.CheckingAccount.Queries.GetBalance
{
    public class GetBalanceHandler : IQueryHandler<GetBalanceQuery, GetBalanceResponse>
    {
        private readonly ICheckingAccountService _service;

        public GetBalanceHandler(ICheckingAccountService service)
        {
            _service = service;
        }

        public async Task<Result<GetBalanceResponse>> Handle(GetBalanceQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var result = _service.GetAccountInfo(query.AccountId);

                if (result == null)
                {
                    return Result.Failure<GetBalanceResponse>(CheckingAccountErrors.InvalidBalanceInfoResult);
                }

                return Result.Success(result);
            }
            catch (Exception ex)
            {
                return Result.Failure<GetBalanceResponse>(Error.Problem("INVALID_RESULT", ex.Message));
            }
        }
    }
}

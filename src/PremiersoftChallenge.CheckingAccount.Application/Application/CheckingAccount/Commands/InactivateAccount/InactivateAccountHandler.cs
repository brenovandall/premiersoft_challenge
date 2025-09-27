using Application.Authentication;
using Application.Data.Repository;
using PremiersoftChallenge.BuildingBlocks.CQRS;
using PremiersoftChallenge.BuildingBlocks.Errors;
using PremiersoftChallenge.BuildingBlocks.Results;
using PremiersoftChallenge.SharedKernel.Exceptions;

namespace Application.CheckingAccount.Commands.InactivateAccount
{
    public class InactivateAccountHandler : ICommandHandler<InactivateAccountCommand, bool>
    {
        private readonly ICheckingAccountRepository _repository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILoggedContext _loggedContext;

        public InactivateAccountHandler(
            ICheckingAccountRepository repository,
            IPasswordHasher passwordHasher,
            ILoggedContext loggedContext)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
            _loggedContext = loggedContext;
        }

        public async Task<Result<bool>> Handle(InactivateAccountCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var account = _repository.GetById(_loggedContext.Id);
                if (account == null)
                {
                    return Result.Failure<bool>(CheckingAccountErrors.InvalidCredentials);
                }

                var verified = _passwordHasher.Verify(command.Password, account.Password, account.Salt);
                if (!verified)
                {
                    return Result.Failure<bool>(CheckingAccountErrors.InvalidCredentials);
                }

                _repository.Update(account.Inactivate());

                return Result.Success(true);
            }
            catch (DomainException ex)
            {
                return Result.Failure<bool>(Error.Problem("BUSINESS_ERROR", ex.Message));
            }
        }
    }
}

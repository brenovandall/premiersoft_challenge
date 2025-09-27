using Application.Authentication;
using Application.Data.Repository;
using PremiersoftChallenge.BuildingBlocks.CQRS;
using PremiersoftChallenge.BuildingBlocks.Errors;
using PremiersoftChallenge.BuildingBlocks.Results;

namespace Application.CheckingAccount.Commands.LoginCheckingAccount
{
    public class LoginCheckingAccountHandler : ICommandHandler<LoginCheckingAccountCommand, string>
    {
        private readonly ICheckingAccountRepository _repository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenProvider _tokenProvider;

        public LoginCheckingAccountHandler(
            ICheckingAccountRepository repository,
            IPasswordHasher passwordHasher,
            ITokenProvider tokenProvider)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
            _tokenProvider = tokenProvider;
        }

        public async Task<Result<string>> Handle(LoginCheckingAccountCommand command, CancellationToken cancellationToken)
        {
            var account = _repository.GetByAccountNumberOrName(command.Identifier);
            if (account == null)
            {
                return Result.Failure<string>(CheckingAccountErrors.InvalidCredentials);
            }

            var verified = _passwordHasher.Verify(command.Password, account.Password, account.Salt);
            if (!verified)
            {
                return Result.Failure<string>(CheckingAccountErrors.InvalidCredentials);
            }

            return _tokenProvider.Create(account);
        }
    }
}

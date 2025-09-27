using Application.Authentication;
using PremiersoftChallenge.BuildingBlocks.CQRS;
using PremiersoftChallenge.BuildingBlocks.Errors;
using PremiersoftChallenge.BuildingBlocks.Results;
using PremiersoftChallenge.BuildingBlocks.Validators;

namespace Application.CheckingAccount.Commands.CreateCheckingAccount
{
    public class CreateCheckingAccountHandler : ICommandHandler<CreateCheckingAccountCommand, CreateCheckingAccountResult>
    {
        private readonly IPasswordHasher _passwordHasher;

        public CreateCheckingAccountHandler(IPasswordHasher passwordHasher)
        {
            _passwordHasher = passwordHasher;
        }

        public async Task<Result<CreateCheckingAccountResult>> Handle(CreateCheckingAccountCommand command, CancellationToken cancellationToken)
        {
            if (!CpfValidator.IsCpfValid(command.Cpf))
                return Result.Failure<CreateCheckingAccountResult>(CheckingAccountErrors.InvalidCpf);

            var passwordHash = _passwordHasher.Hash(command.Password);
            var parts = passwordHash.Split('-');
            var password = parts[0];
            var salt = parts[1];
            var checkingAccount = Domain.CheckingAccount.Create(command.Cpf, password, salt);

            // todo - salva no banco

            return new CreateCheckingAccountResult(checkingAccount.Number);
        }
    }
}

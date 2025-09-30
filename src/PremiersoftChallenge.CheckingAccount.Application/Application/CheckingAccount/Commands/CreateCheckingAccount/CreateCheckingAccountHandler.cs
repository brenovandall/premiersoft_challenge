using Application.Authentication;
using Application.Data.Repository;
using PremiersoftChallenge.BuildingBlocks.CQRS;
using PremiersoftChallenge.BuildingBlocks.Errors;
using PremiersoftChallenge.BuildingBlocks.Results;
using PremiersoftChallenge.BuildingBlocks.Validators;

namespace Application.CheckingAccount.Commands.CreateCheckingAccount
{
    public class CreateCheckingAccountHandler : ICommandHandler<CreateCheckingAccountCommand, CreateCheckingAccountResult>
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICheckingAccountRepository _repository;

        public CreateCheckingAccountHandler(IPasswordHasher passwordHasher, ICheckingAccountRepository repository)
        {
            _passwordHasher = passwordHasher;
            _repository = repository;
        }

        public async Task<Result<CreateCheckingAccountResult>> Handle(CreateCheckingAccountCommand command, CancellationToken cancellationToken)
        {
            if (!CpfValidator.IsCpfValid(command.Cpf))
                return Result.Failure<CreateCheckingAccountResult>(CheckingAccountErrors.InvalidCpf);

            var passwordHash = _passwordHasher.Hash(command.Password);
            var parts = passwordHash.Split('-');
            var password = parts[0];
            var salt = parts[1];
            var number = await _repository.Count() + 1;
            var checkingAccount = Domain.CheckingAccount.Create(number, command.Cpf, password, salt);

            await _repository.Add(checkingAccount);

            return new CreateCheckingAccountResult(checkingAccount.Number);
        }
    }
}

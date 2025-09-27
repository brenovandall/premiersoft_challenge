using PremiersoftChallenge.BuildingBlocks.CQRS;

namespace Application.CheckingAccount.Commands.CreateCheckingAccount
{
    public sealed record CreateCheckingAccountCommand(string Cpf, string Password) : ICommand<CreateCheckingAccountResult>;
    public sealed record CreateCheckingAccountResult(long AccountNumber);
}

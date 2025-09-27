using PremiersoftChallenge.BuildingBlocks.CQRS;

namespace Application.CheckingAccount.Commands.InactivateAccount
{
    public sealed record InactivateAccountCommand(string Password) : ICommand<bool>;
}

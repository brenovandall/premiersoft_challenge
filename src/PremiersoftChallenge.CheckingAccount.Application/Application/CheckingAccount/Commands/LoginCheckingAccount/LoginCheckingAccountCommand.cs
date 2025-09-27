using PremiersoftChallenge.BuildingBlocks.CQRS;

namespace Application.CheckingAccount.Commands.LoginCheckingAccount
{
    public sealed record LoginCheckingAccountCommand(string Identifier, string Password) : ICommand<string>;
}

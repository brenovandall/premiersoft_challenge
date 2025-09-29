using Api.Extensions;
using Api.Infrastructure;
using Application.CheckingAccount.Commands.LoginCheckingAccount;
using Carter;
using PremiersoftChallenge.BuildingBlocks.CQRS;

namespace Api.Endpoints
{
    public sealed class LoginCheckingAccount : ICarterModule
    {
        public sealed record LoginCheckingAccountRequest(string Identifier, string Password);

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/v1/checkingAccount/login", async (
                LoginCheckingAccountRequest request,
                ICommandHandler<LoginCheckingAccountCommand, string> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new LoginCheckingAccountCommand(
                    Identifier: request.Identifier, Password: request.Password);

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithName("Login")
            .Produces<string>(StatusCodes.Status200OK)
            .WithSummary("Realizar o login em uma conta corrente.")
            .WithDescription("Endpoint responsável por autenticar um cliente da conta corrente. " +
                             "Recebe credenciais de acesso (CPF/número da conta e senha), valida com a camada de aplicação " +
                             "e retorna um token válido por 15 minutos em caso de autenticação bem-sucedida. " +
                             "Caso contrário, retorna os erros apropriados de autenticação.");
        }
    }
}

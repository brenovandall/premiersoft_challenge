using Api.Extensions;
using Application.CheckingAccount.Commands.CreateCheckingAccount;
using Carter;
using PremiersoftChallenge.BuildingBlocks.CQRS;

namespace Api.Endpoints
{
    public sealed class CreateCheckingAccount : ICarterModule
    {
        public sealed record CreateCheckingAccountRequest(string Cpf, string Password);

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/v1/checkingAccount/createAccount", async (
                CreateCheckingAccountRequest request,
                ICommandHandler<CreateCheckingAccountCommand, CreateCheckingAccountResult> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new CreateCheckingAccountCommand(Cpf: request.Cpf, Password: request.Password);

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithName("CreateAccount")
            .Produces<CreateCheckingAccountResult>(StatusCodes.Status201Created)
            .WithSummary("Cria uma nova conta corrente.")
            .WithDescription("Recebe os dados de CPF e senha, cria uma nova conta corrente e retorna o número da conta criada. " +
                             "Caso haja falha de validação ou regra de negócio, retorna detalhes do problema.");
        }
    }
}

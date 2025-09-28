using Api.Extensions;
using Application.CheckingAccount.Commands.InactivateAccount;
using Carter;
using PremiersoftChallenge.BuildingBlocks.CQRS;

namespace Api.Endpoints
{
    public sealed class InactivateAccount : ICarterModule
    {
        public sealed record InactivateAccountRequest(string Password);

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/v1/checkingAccount/inactivate", async (
                InactivateAccountRequest request,
                ICommandHandler<InactivateAccountCommand, bool> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new InactivateAccountCommand(Password: request.Password);

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .WithName("Inactivate")
            .RequireAuthorization()
            .Produces<bool>(StatusCodes.Status204NoContent)
            .WithSummary("Inativar uma conta corrente.")
            .WithDescription("Endpoint responsável por inativar a conta corrente logada. " +
                             "Recebe a senha como parâmetro para validar a operação. ");
        }
    }
}

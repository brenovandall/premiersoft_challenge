using Api.Extensions;
using Application.Transaction.Commands.MakeTransaction;
using Carter;
using PremiersoftChallenge.BuildingBlocks.CQRS;

namespace Api.Endpoints
{
    public sealed class MakeTransaction : ICarterModule
    {
        public sealed record MakeTransactionRequest(long? AccountNumber, double Value, string TransactionFlow);

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/v1/transaction/perform", async (
                MakeTransactionRequest request,
                ICommandHandler<MakeTransactionCommand, bool> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new MakeTransactionCommand(AccountNumber: request.AccountNumber,
                    Value: request.Value, TransactionFlow: request.TransactionFlow);

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .WithName("PerformTransaction")
            .RequireAuthorization()
            .Produces<string>(StatusCodes.Status204NoContent);
        }
    }
}

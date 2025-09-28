using Api.Extensions;
using Application.Transaction.Commands.MakeTransaction;
using Carter;
using PremiersoftChallenge.BuildingBlocks.CQRS;

namespace Api.Endpoints
{
    public sealed class MakeTransaction : ICarterModule
    {
        public sealed record MakeTransactionRequest(string RequestId, long? AccountNumber, double Value, string TransactionFlow);

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/v1/transaction/perform", async (
                MakeTransactionRequest request,
                ICommandHandler<MakeTransactionCommand, bool> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new MakeTransactionCommand(
                    RequestId: request.RequestId, AccountNumber: request.AccountNumber,
                    Value: request.Value, TransactionFlow: request.TransactionFlow);

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .WithName("PerformTransaction")
            .RequireAuthorization()
            .Produces<bool>(StatusCodes.Status204NoContent)
            .WithSummary("Realizar transação na conta corrente")
            .WithDescription("Executa uma transação de crédito ou débito na conta corrente informada ou autenticada. " +
                             "Requer autenticação e utiliza os dados enviados na requisição (número da conta, valor e tipo de transação). " +
                             "Retorna **204 No Content** em caso de sucesso ou mensagem de erro em caso de falha.");
        }
    }
}

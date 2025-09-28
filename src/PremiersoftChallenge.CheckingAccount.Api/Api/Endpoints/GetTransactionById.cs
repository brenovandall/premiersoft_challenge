using Api.Extensions;
using Application.Transaction.Queries.GetTransactionById;
using Carter;
using PremiersoftChallenge.BuildingBlocks.CQRS;

namespace Api.Endpoints
{
    public sealed class GetTransactionById : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/v1/transaction/{transactionId}", async (
                string transactionId,
                IQueryHandler<GetTransactionByIdQuery, GetTransactionByIdResult> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new GetTransactionByIdQuery(TransactionId: transactionId);

                var result = await handler.Handle(query, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithName("TransactionInfo")
            .RequireAuthorization()
            .Produces<GetTransactionByIdResult>(StatusCodes.Status200OK)
            .WithSummary("Obter informações detalhadas de uma transação.")
            .WithDescription("Este endpoint retorna os **detalhes completos de uma transação** a partir do seu identificador (`transactionId`). " +
                             "Caso a transação não seja encontrada ou ocorra algum erro de processamento, será retornada uma resposta padronizada de erro.");
        }
    }
}

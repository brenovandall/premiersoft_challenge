using Api.Extensions;
using Application.Authentication;
using Application.CheckingAccount.Queries.GetBalance;
using Carter;
using PremiersoftChallenge.BuildingBlocks.CQRS;

namespace Api.Endpoints
{
    public sealed class GetBalanceInfo : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/v1/checkingAccount/balanceInfo", async (
                IQueryHandler<GetBalanceQuery, GetBalanceResponse> handler,
                ILoggedContext loggedContext,
                CancellationToken cancellationToken) =>
            {
                var query = new GetBalanceQuery(AccountId: loggedContext.Id);

                var result = await handler.Handle(query, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithName("BalanceInfo")
            .RequireAuthorization()
            .Produces<GetBalanceResponse>(StatusCodes.Status200OK)
            .WithSummary("Obter informações de saldo da conta corrente.")
            .WithDescription("Retorna as informações de saldo da conta corrente do cliente autenticado. " +
                             "Utiliza o identificador do cliente logado para buscar os dados. " +
                             "Responde com **200 OK** em caso de sucesso ou mensagem de erro em caso de falha.");
        }
    }
}

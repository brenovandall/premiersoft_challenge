using Api.Extensions;
using Application.CheckingAccount.Queries.GetCheckingAccountIdByNumber;
using Carter;
using PremiersoftChallenge.BuildingBlocks.CQRS;

namespace Api.Endpoints
{
    public sealed class GetCheckingAccountIdByNumber : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/v1/checkingAccount/{accountNumber}", async (
                long accountNumber,
                IQueryHandler<GetCheckingAccountIdByNumberQuery, GetCheckingAccountIdByNumberResult> handler,
                CancellationToken cancellationToken) =>
            {
                var query = new GetCheckingAccountIdByNumberQuery(AccountNumber: accountNumber);

                var result = await handler.Handle(query, cancellationToken);

                return result.Match(Results.Ok, CustomResults.Problem);
            })
            .WithName("CheckingAccountId")
            .RequireAuthorization()
            .Produces<GetCheckingAccountIdByNumberResult>(StatusCodes.Status200OK)
            .WithSummary("Obter o identificador da conta corrente pelo número.")
            .WithDescription("Este endpoint retorna o **ID da conta corrente** associado ao número informado. " +
                             "Caso não seja encontrada uma conta correspondente, será retornado um erro padronizado.");
        }
    }
}

using Api.Extensions;
using Api.Infrastructure;
using Application.Transfer.Commands.MakeTransfer;
using Carter;
using PremiersoftChallenge.BuildingBlocks.CQRS;

namespace Api.Endpoints
{
    public sealed class MakeTransfer : ICarterModule
    {
        public sealed record MakeTransferRequest(long TargetAccountNumber, double Value);

        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/v1/transfer/perform", async (
                MakeTransferRequest request,
                ICommandHandler<MakeTransferCommand, bool> handler,
                CancellationToken cancellationToken) =>
            {
                var command = new MakeTransferCommand(
                    TargetAccountNumber: request.TargetAccountNumber, Value: request.Value);

                var result = await handler.Handle(command, cancellationToken);

                return result.Match(Results.NoContent, CustomResults.Problem);
            })
            .WithName("CreateAccount")
            .Produces<bool>(StatusCodes.Status204NoContent)
            .AddEndpointFilter<IdempotencyFilter>()
            .WithSummary("Realizar uma transferência entre contas correntes.")
            .WithDescription("Este endpoint executa a **transferência de valores** para a conta de destino informada. " +
                             "É necessário fornecer o número da conta de destino (`targetAccountNumber`) e o valor da transferência (`value`). " +
                             "Se a operação for concluída com sucesso, retorna um status `204 No Content`. " +
                             "Caso ocorra erro de validação ou de negócio, será retornada uma resposta de problema padronizada.");
        }
    }
}

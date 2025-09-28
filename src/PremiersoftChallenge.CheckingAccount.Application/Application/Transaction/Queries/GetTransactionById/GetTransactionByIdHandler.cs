using Application.Services;
using PremiersoftChallenge.BuildingBlocks.CQRS;
using PremiersoftChallenge.BuildingBlocks.Results;

namespace Application.Transaction.Queries.GetTransactionById
{
    public class GetTransactionByIdHandler : IQueryHandler<GetTransactionByIdQuery, GetTransactionByIdResult>
    {
        private readonly ITransactionService _transactionService;

        public GetTransactionByIdHandler(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task<Result<GetTransactionByIdResult>> Handle(GetTransactionByIdQuery query, CancellationToken cancellationToken)
        {
            var valid = Guid.TryParse(query.TransactionId, out var parsedTransactionId);
            if (!valid)
            {
                return Result.Failure<GetTransactionByIdResult>(
                    Error.Failure("INVALID_OPERATION", $"Não foi possível converter o valor {parsedTransactionId} para o formato esperado."));
            }

            var transaction = _transactionService.GetById(parsedTransactionId);
            if (transaction == null)
            {
                return Result.Failure<GetTransactionByIdResult>(
                    Error.Problem("TRANSACTION_NOT_FOUND", "A transação não foi encontrada para o identificador fornecido."));
            }

            return Result.Success(new GetTransactionByIdResult(transaction));
        }
    }
}

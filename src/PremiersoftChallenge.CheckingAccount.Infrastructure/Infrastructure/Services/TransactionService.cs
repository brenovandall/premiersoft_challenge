using Application.Services;
using Application.Transaction.Queries.GetTransactionById;
using Infrastructure.Abstractions.Queries;
using Infrastructure.Extensions;

namespace Infrastructure.Services
{
    internal sealed class TransactionService : ITransactionService
    {
        private readonly IQueryExecutorFactory _queryExecutorFactory;

        public TransactionService(IQueryExecutorFactory queryExecutorFactory)
        {
            _queryExecutorFactory = queryExecutorFactory;
        }

        public GetTransactionByIdResponse? GetById(Guid id)
        {
            var sql = @"
SELECT
	idmovimento Id,
	idcontacorrente CheckingAccountId,
	datamovimento TransactionDate,
	tipomovimento TransactionFlow,
	valor Value
FROM movimento WHERE idmovimento = @id";
            var param = new { id };

            return GetSqlQueryFactory().SetQuery(sql).ExecuteFirstOrDefault<GetTransactionByIdResponse?>(param);
        }

        private IQueryExecutor GetSqlQueryFactory()
        {
            var factory = _queryExecutorFactory.Create(OrmsProviders.Dapper);

            if (factory == null)
                throw new Exception(nameof(factory));

            return factory;
        }
    }
}

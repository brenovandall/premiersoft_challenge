using Application.Services;
using Application.Transaction.Queries.GetTransactionById;
using PremiersoftChallenge.Data;
using PremiersoftChallenge.Data.Abstractions.Queries;

namespace Infrastructure.Services
{
    internal sealed class TransactionService : ITransactionService
    {
        private readonly IQueryExecutorFactory _queryExecutorFactory;

        public TransactionService(IQueryExecutorFactory queryExecutorFactory)
        {
            _queryExecutorFactory = queryExecutorFactory;
        }

        public async Task<GetTransactionByIdResponse?> GetById(Guid id)
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

            return await GetSqlQueryFactory().SetQuery(sql).ExecuteFirstOrDefaultAsync<GetTransactionByIdResponse?>(param);
        }

        private IQueryExecutor GetSqlQueryFactory()
        {
            var factory = _queryExecutorFactory.Create(OrmProviders.Dapper, DbStrategies.Sqlite);

            if (factory == null)
                throw new Exception(nameof(factory));

            return factory;
        }
    }
}

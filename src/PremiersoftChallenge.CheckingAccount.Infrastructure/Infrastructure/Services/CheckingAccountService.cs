using Application.CheckingAccount.Queries.GetBalance;
using Application.Services;
using PremiersoftChallenge.Data;
using PremiersoftChallenge.Data.Abstractions.Queries;

namespace Infrastructure.Services
{
    internal sealed class CheckingAccountService : ICheckingAccountService
    {
        private readonly IQueryExecutorFactory _queryExecutorFactory;

        public CheckingAccountService(IQueryExecutorFactory queryExecutorFactory)
        {
            _queryExecutorFactory = queryExecutorFactory;
        }

        public async Task<GetBalanceResponse?> GetAccountInfo(Guid checkingAccountId)
        {
            var sql = @"
SELECT
    a.numero AccountNumber,
    a.nome Name,
    COALESCE(SUM(CASE WHEN m.tipomovimento = 'C' THEN m.valor ELSE 0 END), 0) -
    COALESCE(SUM(CASE WHEN m.tipomovimento = 'D' THEN m.valor ELSE 0 END), 0) Balance
FROM contacorrente a
LEFT JOIN movimento m ON m.idcontacorrente = a.idcontacorrente
WHERE a.idcontacorrente = @id GROUP BY a.numero, a.nome";
            var param = new { id = checkingAccountId.ToString().ToUpper() };

            return await GetSqlQueryFactory().SetQuery(sql).ExecuteFirstOrDefaultAsync<GetBalanceResponse?>(param);
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

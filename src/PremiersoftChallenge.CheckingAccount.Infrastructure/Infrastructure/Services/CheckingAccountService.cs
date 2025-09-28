using Application.CheckingAccount.Queries.GetBalance;
using Application.Services;
using Infrastructure.Abstractions.Queries;
using Infrastructure.Extensions;

namespace Infrastructure.Services
{
    internal sealed class CheckingAccountService : ICheckingAccountService
    {
        private readonly IQueryExecutorFactory _queryExecutorFactory;

        public CheckingAccountService(IQueryExecutorFactory queryExecutorFactory)
        {
            _queryExecutorFactory = queryExecutorFactory;
        }

        public GetBalanceResponse? GetAccountInfo(Guid checkingAccountId)
        {
            var sql = @"
SELECT
    a.numero AccountNumber,
    a.nome Name,
    COALESCE(SUM(CASE WHEN m.tipomovimento = 'C' THEN m.valor ELSE 0 END), 0) -
    COALESCE(SUM(CASE WHEN m.tipomovimento = 'D' THEN m.valor ELSE 0 END), 0) Balance
FROM contacorrente a
JOIN movimento m ON m.idcontacorrente = a.idcontacorrente
GROUP BY a.numero, a.nome";
            var param = new { id = checkingAccountId };

            return GetSqlQueryFactory().SetQuery(sql).ExecuteFirstOrDefault<GetBalanceResponse?>(param);
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

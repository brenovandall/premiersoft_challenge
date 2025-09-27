using Application.Data.Repository;
using Domain;
using Infrastructure.Abstractions.Commands;
using Infrastructure.Abstractions.Queries;
using Infrastructure.Extensions;

namespace Infrastructure.Repository
{
    internal sealed class CheckingAccountRepository : ICheckingAccountRepository
    {
        private readonly ISqlRawCommandFactory _sqlRawCommandFactory;
        private readonly IQueryExecutorFactory _queryExecutorFactory;

        public CheckingAccountRepository(ISqlRawCommandFactory sqlRawCommandFactory, IQueryExecutorFactory queryExecutorFactory)
        {
            _sqlRawCommandFactory = sqlRawCommandFactory;
            _queryExecutorFactory = queryExecutorFactory;
        }

        public void Add(ICheckingAccount checkingAccount)
        {
            var sql = "INSERT INTO contacorrente VALUES (@id, @number, @name, @active, @password, @salt)";
            var param = new
            {
                id = checkingAccount.Id,
                number = checkingAccount.Number,
                name = checkingAccount.Name,
                active = (int)checkingAccount.Status,
                password = checkingAccount.Password,
                salt = checkingAccount.Salt
            };

            GetSqlCommandFactory().SetCommand(sql).Execute(param);
        }

        public ICheckingAccount? GetByAccountNumberOrName(string searchString)
        {
            var sql = @"
SELECT
	idcontacorrente Id,
	numero Number,
	nome Name,
	ativo Status,
	senha Password,
	salt Salt
FROM contacorrente WHERE nome = @name OR numero = @number";
            var param = new
            {
                name = searchString,
                number = searchString
            };

            return GetSqlQueryFactory().SetQuery(sql).ExecuteFirstOrDefault<CheckingAccount>(param);
        }

        private IQueryExecutor GetSqlQueryFactory()
        {
            var factory = _queryExecutorFactory.Create(OrmsProviders.Dapper);

            if (factory == null)
                throw new Exception(nameof(factory));

            return factory;
        }

        private ISqlRawCommand GetSqlCommandFactory()
        {
            var factory = _sqlRawCommandFactory.Create(OrmsProviders.Dapper);

            if (factory == null)
                throw new Exception(nameof(factory));

            return factory;
        }
    }
}

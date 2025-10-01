using Application.Data.Repository;
using Domain;
using Domain.Enums;
using PremiersoftChallenge.Data;
using PremiersoftChallenge.Data.Abstractions.Commands;
using PremiersoftChallenge.Data.Abstractions.Queries;

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

        public async Task Add(ICheckingAccount checkingAccount)
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

            await GetSqlCommandFactory().SetCommand(sql).ExecuteAsync(param);
        }

        public async Task<long> MaxAccountNumber()
        {
            var sql = "SELECT MAX(numero) Result FROM contacorrente";

            return await GetSqlQueryFactory().SetQuery(sql).ExecuteFirstOrDefaultAsync<long>();
        }

        public async Task<ICheckingAccount?> GetById(Guid id)
        {
            var sql = @"
SELECT
	idcontacorrente Id,
	numero Number,
	nome Name,
	ativo Status,
	senha Password,
	salt Salt
FROM contacorrente WHERE idcontacorrente = @id";
            var param = new { id };

            return await GetSqlQueryFactory().SetQuery(sql).ExecuteFirstOrDefaultAsync<CheckingAccount>(param);
        }

        public async Task<ICheckingAccount?> GetByAccountNumberOrName(string searchString)
        {
            var sql = @"
SELECT
	idcontacorrente Id,
	numero Number,
	nome Name,
	ativo Status,
	senha Password,
	salt Salt
FROM contacorrente WHERE (nome = @name OR numero = @number) AND ativo = @active";
            var param = new
            {
                name = searchString,
                number = searchString,
                active = (int)CheckingAccountStatus.Active
            };

            return await GetSqlQueryFactory().SetQuery(sql).ExecuteFirstOrDefaultAsync<CheckingAccount>(param);
        }

        public async Task Update(ICheckingAccount checkingAccount)
        {
            var sql = @"UPDATE contacorrente SET numero = @number, nome = @name, ativo = @active, senha = @password,
salt = @salt WHERE idcontacorrente = @id";
            var param = new
            {
                id = checkingAccount.Id,
                number = checkingAccount.Number,
                name = checkingAccount.Name,
                active = (int)checkingAccount.Status,
                password = checkingAccount.Password,
                salt = checkingAccount.Salt
            };

            await GetSqlCommandFactory().SetCommand(sql).ExecuteAsync(param);
        }

        private IQueryExecutor GetSqlQueryFactory()
        {
            var factory = _queryExecutorFactory.Create(OrmProviders.Dapper, DbStrategies.Sqlite);

            if (factory == null)
                throw new Exception(nameof(factory));

            return factory;
        }

        private ISqlRawCommand GetSqlCommandFactory()
        {
            var factory = _sqlRawCommandFactory.Create(OrmProviders.Dapper, DbStrategies.Sqlite);

            if (factory == null)
                throw new Exception(nameof(factory));

            return factory;
        }
    }
}

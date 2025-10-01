using Application.Dto;
using Application.Services;
using PremiersoftChallenge.Data;
using PremiersoftChallenge.Data.Abstractions.Commands;
using PremiersoftChallenge.Data.Abstractions.Queries;

namespace Infrastructure.Services
{
    internal sealed class IdempotencyService : IIdempotencyService
    {
        private readonly ISqlRawCommandFactory _sqlRawCommandFactory;
        private readonly IQueryExecutorFactory _queryExecutorFactory;

        public IdempotencyService(ISqlRawCommandFactory sqlRawCommandFactory, IQueryExecutorFactory queryExecutorFactory)
        {
            _sqlRawCommandFactory = sqlRawCommandFactory;
            _queryExecutorFactory = queryExecutorFactory;
        }

        public async Task Add(IdempotentDto dto)
        {
            var sql = "INSERT INTO idempotencia VALUES (@key, @request, @response)";
            var param = new
            {
                key = dto.Key,
                request = dto.Request,
                response = dto.Response
            };

            await GetSqlCommandFactory().SetCommand(sql).ExecuteAsync(param);
        }

        public async Task<IdempotentDto?> GetByKey(string key)
        {
            var sql = @"
SELECT
    chave_idempotencia Key,
    requisicao Request,
    resultado Response
FROM idempotencia WHERE chave_idempotencia = @key";
            var param = new { key };

            return await GetSqlQueryFactory().SetQuery(sql).ExecuteFirstOrDefaultAsync<IdempotentDto?>(param);
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

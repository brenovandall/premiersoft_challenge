using Application.Dto;
using Application.Services;
using Infrastructure.Abstractions.Commands;
using Infrastructure.Abstractions.Queries;
using Infrastructure.Extensions;

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

        public void Add(IdempotentDto dto)
        {
            var sql = "INSERT INTO idempotencia VALUES (@key, @request, @response)";
            var param = new
            {
                key = dto.Key,
                request = dto.Request,
                response = dto.Response
            };

            GetSqlCommandFactory().SetCommand(sql).Execute(param);
        }

        public IdempotentDto? GetByKeyAndRequest(string key, string request)
        {
            var sql = @"
SELECT
    chave_idempotencia Key,
    requisicao Request,
    resultado Response
FROM idempotencia WHERE chave_idempotencia = @key AND requisicao = @request";
            var param = new { key, request };

            return GetSqlQueryFactory().SetQuery(sql).ExecuteFirstOrDefault<IdempotentDto?>(param);
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

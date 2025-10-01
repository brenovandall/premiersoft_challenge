using Dapper;
using Infrastructure.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using PremiersoftChallenge.Data;
using PremiersoftChallenge.Data.Abstractions.Queries;
using System.Data;

namespace Infrastructure.Data
{
    internal sealed class SqliteQueryExecutor : IQueryExecutor
    {
        public string OrmProvider => OrmProviders.Dapper;
        public string Strategy => DbStrategies.Sqlite;

        private readonly IConfiguration _configuration;
        private string _sql = default!;

        public SqliteQueryExecutor(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public T? ExecuteFirstOrDefault<T>(object? parameters = null, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var closed = connection == null;
            connection = connection == null ? new SqliteConnection(_configuration.GetConnectionString("DbConnection")) : connection;

            if (closed)
            {
                connection.Open();
            }

            return connection.QueryFirstOrDefault<T>(_sql, parameters, transaction);
        }

        public async Task<T?> ExecuteFirstOrDefaultAsync<T>(object? parameters = null, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var closed = connection == null;
            connection = connection == null ? new SqliteConnection(_configuration.GetConnectionString("DbConnection")) : connection;

            var taskConn = (SqliteConnection)connection;

            if (closed)
            {
                await taskConn.OpenAsync();
            }

            return await taskConn.QueryFirstOrDefaultAsync<T>(_sql, parameters, transaction);
        }

        public IEnumerable<T> Fetch<T>(object? parameters = null, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var closed = connection == null;
            connection = connection == null ? new SqliteConnection(_configuration.GetConnectionString("DbConnection")) : connection;

            if (closed)
            {
                connection.Open();
            }

            return connection.Query<T>(_sql, parameters, transaction);
        }

        public IQueryExecutor SetQuery(string sqlQuery)
        {
            _sql = sqlQuery;
            return this;
        }
    }
}

using Dapper;
using Infrastructure.Extensions;
using Microsoft.Data.Sqlite;
using System.Data;

namespace Infrastructure.Abstractions.Queries
{
    internal sealed class DapperQueryExecutor : IQueryExecutor
    {
        public string Strategy => OrmsProviders.Dapper;

        private string _sql = default!;

        public T? ExecuteFirstOrDefault<T>(object? parameters = null, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var closed = connection == null;
            connection = connection == null ? new SqliteConnection(ConnectionStringBuilder.GetConnectionString()) : connection;

            if (closed)
            {
                connection.Open();
            }

            return connection.QueryFirstOrDefault<T>(_sql, parameters, transaction);
        }

        public Task<T?> ExecuteFirstOrDefaultAsync<T>(object? parameters = null, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> Fetch<T>(object? parameters = null, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var closed = connection == null;
            connection = connection == null ? new SqliteConnection(ConnectionStringBuilder.GetConnectionString()) : connection;

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

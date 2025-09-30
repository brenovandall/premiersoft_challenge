using Dapper;
using Infrastructure.Extensions;
using Microsoft.Data.Sqlite;
using PremiersoftChallenge.Data;
using PremiersoftChallenge.Data.Abstractions.Commands;
using System.Data;

namespace Infrastructure.Data
{
    internal sealed class SqliteSqlRawCommand : ISqlRawCommand
    {
        public string OrmProvider => OrmProviders.Dapper;
        public string Strategy => DbStrategies.Sqlite;

        private string _sql = default!;

        public int? Execute(object? parameters = null, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var closed = connection == null;
            connection = connection == null ? new SqliteConnection(ConnectionStringBuilder.GetConnectionString()) : connection;

            if (closed)
            {
                connection.Open();
            }

            return connection.Execute(_sql, parameters, transaction);
        }

        public async Task<int?> ExecuteAsync(object? parameters = null, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            var closed = connection == null;
            connection = connection == null ? new SqliteConnection(ConnectionStringBuilder.GetConnectionString()) : connection;

            var taskConn = (SqliteConnection)connection;

            if (closed)
            {
                await taskConn.OpenAsync();
            }

            return await taskConn.ExecuteAsync(_sql, parameters, transaction);
        }

        public ISqlRawCommand SetCommand(string sqlCommand)
        {
            _sql = sqlCommand;

            return this;
        }
    }
}

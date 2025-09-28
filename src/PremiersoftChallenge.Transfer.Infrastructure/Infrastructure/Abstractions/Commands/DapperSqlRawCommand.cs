using Dapper;
using Infrastructure.Extensions;
using Microsoft.Data.Sqlite;
using System.Data;

namespace Infrastructure.Abstractions.Commands
{
    internal sealed class DapperSqlRawCommand : ISqlRawCommand
    {
        public string Strategy => OrmsProviders.Dapper;

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

        public Task<int?> ExecuteAsync(object? parameters = null, IDbConnection? connection = null, IDbTransaction? transaction = null)
        {
            throw new NotImplementedException();
        }

        public ISqlRawCommand SetCommand(string sqlCommand)
        {
            _sql = sqlCommand;

            return this;
        }
    }
}

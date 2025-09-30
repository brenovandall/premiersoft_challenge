using System.Data;

namespace PremiersoftChallenge.Data.Abstractions.Commands
{
    public interface ISqlRawCommand
    {
        string OrmProvider { get; }
        string Strategy { get; }

        int? Execute(object? parameters = null, IDbConnection? connection = null, IDbTransaction? transaction = null);
        Task<int?> ExecuteAsync(object? parameters = null, IDbConnection? connection = null, IDbTransaction? transaction = null);
        ISqlRawCommand SetCommand(string sqlCommand);
    }
}

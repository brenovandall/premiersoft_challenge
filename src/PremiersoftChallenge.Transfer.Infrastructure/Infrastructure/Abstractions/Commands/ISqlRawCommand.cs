using System.Data;

namespace Infrastructure.Abstractions.Commands
{
    public interface ISqlRawCommand
    {
        string Strategy { get; }

        int? Execute(object? parameters = null, IDbConnection? connection = null, IDbTransaction? transaction = null);
        Task<int?> ExecuteAsync(object? parameters = null, IDbConnection? connection = null, IDbTransaction? transaction = null);
        ISqlRawCommand SetCommand(string sqlCommand);
    }
}

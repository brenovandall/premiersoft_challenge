using System.Data;

namespace PremiersoftChallenge.Data.Abstractions.Queries
{
    public interface IQueryExecutor
    {
        string OrmProvider { get; }
        string Strategy { get; }

        IEnumerable<T> Fetch<T>(object? parameters = null, IDbConnection? connection = null, IDbTransaction? transaction = null);
        T? ExecuteFirstOrDefault<T>(object? parameters = null, IDbConnection? connection = null, IDbTransaction? transaction = null);
        Task<T?> ExecuteFirstOrDefaultAsync<T>(object? parameters = null, IDbConnection? connection = null, IDbTransaction? transaction = null);
        IQueryExecutor SetQuery(string sqlQuery);
    }
}

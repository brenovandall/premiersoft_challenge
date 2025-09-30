namespace PremiersoftChallenge.Data.Abstractions.Queries
{
    public interface IQueryExecutorFactory
    {
        IQueryExecutor? Create(string ormProvider, string strategy);
    }
}

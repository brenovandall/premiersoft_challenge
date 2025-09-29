namespace Infrastructure.Abstractions.Queries
{
    public interface IQueryExecutorFactory
    {
        IQueryExecutor? Create(string strategy);
    }
}

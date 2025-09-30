namespace PremiersoftChallenge.Data.Abstractions.Queries
{
    public sealed class QueryExecutorFactory : IQueryExecutorFactory
    {
        private readonly IEnumerable<IQueryExecutor> _strategies;

        public QueryExecutorFactory(IEnumerable<IQueryExecutor> strategies)
        {
            _strategies = strategies;
        }

        public IQueryExecutor? Create(string ormProvider, string strategy)
        {
            return _strategies.FirstOrDefault(s => s.OrmProvider == ormProvider && s.Strategy == strategy);
        }
    }
}

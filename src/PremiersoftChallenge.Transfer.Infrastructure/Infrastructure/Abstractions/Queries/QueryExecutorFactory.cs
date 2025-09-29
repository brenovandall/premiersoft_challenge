using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Abstractions.Queries
{
    internal sealed class QueryExecutorFactory : IQueryExecutorFactory
    {
        private readonly IEnumerable<IQueryExecutor> _strategies;

        public QueryExecutorFactory(IEnumerable<IQueryExecutor> strategies)
        {
            _strategies = strategies;
        }

        public IQueryExecutor? Create(string strategy)
        {
            return _strategies.FirstOrDefault(s => s.Strategy == strategy);
        }
    }
}

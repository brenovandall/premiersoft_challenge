namespace PremiersoftChallenge.Data.Abstractions.Commands
{
    public sealed class SqlRawCommandFactory : ISqlRawCommandFactory
    {
        private readonly IEnumerable<ISqlRawCommand> _strategies;

        public SqlRawCommandFactory(IEnumerable<ISqlRawCommand> strategies)
        {
            _strategies = strategies;
        }

        public ISqlRawCommand? Create(string ormProvider, string strategy)
        {
            return _strategies.FirstOrDefault(s => s.OrmProvider == ormProvider && s.Strategy == strategy);
        }
    }
}

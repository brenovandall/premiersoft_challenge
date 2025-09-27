namespace Infrastructure.Abstractions.Commands
{
    internal sealed class SqlRawCommandFactory : ISqlRawCommandFactory
    {
        private readonly IEnumerable<ISqlRawCommand> _strategies;

        public SqlRawCommandFactory(IEnumerable<ISqlRawCommand> strategies)
        {
            _strategies = strategies;
        }

        public ISqlRawCommand? Create(string strategy)
        {
            return _strategies.FirstOrDefault(s => s.Strategy == strategy);
        }
    }
}

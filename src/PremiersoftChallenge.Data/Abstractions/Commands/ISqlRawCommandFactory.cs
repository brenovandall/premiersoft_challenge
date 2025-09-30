namespace PremiersoftChallenge.Data.Abstractions.Commands
{
    public interface ISqlRawCommandFactory
    {
        ISqlRawCommand? Create(string ormProvider, string strategy);
    }
}

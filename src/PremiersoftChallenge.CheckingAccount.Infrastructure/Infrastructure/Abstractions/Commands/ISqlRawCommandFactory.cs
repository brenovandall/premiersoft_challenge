namespace Infrastructure.Abstractions.Commands
{
    public interface ISqlRawCommandFactory
    {
        ISqlRawCommand? Create(string strategy);
    }
}

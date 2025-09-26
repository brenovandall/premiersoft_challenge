using PremiersoftChallenge.BuildingBlocks.Results;

namespace PremiersoftChallenge.BuildingBlocks.CQRS
{
    public interface ICommandHandler<in TCommand>
    where TCommand : ICommand
    {
        Task<Result> Handle(TCommand command, CancellationToken cancellationToken);
    }

    public interface ICommandHandler<in TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
        Task<Result<TResponse>> Handle(TCommand command, CancellationToken cancellationToken);
    }
}

using PremiersoftChallenge.BuildingBlocks.Results;

namespace Infrastructure.Http
{
    public interface IApiCall
    {
        Task<Result<T>> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);
        Task<Result> PostAsync<TRequest>(string endpoint, TRequest body, CancellationToken cancellationToken = default);
        Task<Result<TResponse>> PostAsync<TResponse, TRequest>(string endpoint, TRequest body, CancellationToken cancellationToken = default);
    }
}

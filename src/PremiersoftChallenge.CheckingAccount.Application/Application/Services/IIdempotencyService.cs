using Application.Dto;

namespace Application.Services
{
    public interface IIdempotencyService
    {
        Task Add(IdempotentDto dto);
        Task<IdempotentDto?> GetByKeyAndRequest(string key, string request);
    }
}

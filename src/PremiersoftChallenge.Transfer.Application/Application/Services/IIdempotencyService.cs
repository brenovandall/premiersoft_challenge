using Application.Dto;

namespace Application.Services
{
    public interface IIdempotencyService
    {
        Task Add(IdempotentDto dto);
        Task<IdempotentDto?> GetByKey(string key);
    }
}

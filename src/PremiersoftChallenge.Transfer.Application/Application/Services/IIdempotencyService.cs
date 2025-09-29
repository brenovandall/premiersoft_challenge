using Application.Dto;

namespace Application.Services
{
    public interface IIdempotencyService
    {
        void Add(IdempotentDto dto);
        IdempotentDto? GetByKeyAndRequest(string key, string request);
    }
}

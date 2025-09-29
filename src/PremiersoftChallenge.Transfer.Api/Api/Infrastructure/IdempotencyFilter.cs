using Application.Dto;
using Application.Services;
using System.Text.Json;

namespace Api.Infrastructure
{
    internal sealed class IdempotencyFilter : IEndpointFilter
    {
        private readonly IIdempotencyService _idempotencyService;

        public IdempotencyFilter(IIdempotencyService idempotencyService)
        {
            _idempotencyService = idempotencyService;
        }

        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var httpContext = context.HttpContext;
            var idempotencyKey = httpContext.Request.Headers["Idempotency-Key"].FirstOrDefault();

            if (string.IsNullOrWhiteSpace(idempotencyKey))
            {
                idempotencyKey = Guid.NewGuid().ToString();
            }

            var path = httpContext.Request.Path;
            var persisted = _idempotencyService.GetByKeyAndRequest(idempotencyKey, path);
            if (persisted is not null)
            {
                return persisted.Response;
            }

            object? result = await next(context);

            if (result is IStatusCodeHttpResult { StatusCode: >= 200 and < 300 } and IValueHttpResult valueResult)
            {
                _idempotencyService.Add(
                    GetIdempotentObject(idempotencyKey, path, JsonSerializer.Serialize(valueResult.Value)));
            }

            return result;
        }

        private IdempotentDto GetIdempotentObject(string key, string? request, string response)
        {
            return new IdempotentDto
            {
                Key = key,
                Request = request ?? "",
                Response = response
            };
        }
    }
}

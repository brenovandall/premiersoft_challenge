using Application.Dto;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
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
            var persisted = await _idempotencyService.GetByKey(idempotencyKey);
            if (persisted is not null)
            {
                var requestDeserialized = JsonSerializer.Deserialize<ResultValue>(persisted.Request);

                context.HttpContext.Response.StatusCode = requestDeserialized?.StatusCode ?? 200;

                if (context.HttpContext.Response.StatusCode != 204)
                {
                    var responseJson = JsonSerializer.Serialize(persisted.Response);
                    var responseBytes = System.Text.Encoding.UTF8.GetBytes(responseJson);

                    context.HttpContext.Response.ContentType = "application/json";
                    context.HttpContext.Response.ContentLength = responseBytes.Length;
                    await context.HttpContext.Response.Body.WriteAsync(responseBytes, 0, responseBytes.Length);
                }

                return null;
                //return new ObjectResult(context.HttpContext.Response);
            }

            object? result = await next(context);

            if (result is IStatusCodeHttpResult { StatusCode: >= 200 and < 300 } statusCodeHttpResult)
            {
                var resultValue = new ResultValue(statusCodeHttpResult.StatusCode, path);
                var requestSerialized = JsonSerializer.Serialize(resultValue);
                if (result is IValueHttpResult valueResult)
                {
                    await _idempotencyService.Add(
                        GetIdempotentObject(idempotencyKey, requestSerialized, JsonSerializer.Serialize(valueResult)));
                }
                else
                {
                    await _idempotencyService.Add(GetIdempotentObject(idempotencyKey, requestSerialized, ""));
                }
            }

            return result;
        }

        private static IdempotentDto GetIdempotentObject(string key, string? request, string? response)
        {
            return new IdempotentDto
            {
                Key = key,
                Request = request ?? "",
                Response = response ?? ""
            };
        }

        private sealed class ResultValue
        {
            public int? StatusCode { get; set; }
            public string RequestPath { get; set; } = default!;

            public ResultValue(int? statusCode, string requestPath)
            {
                StatusCode = statusCode;
                RequestPath = requestPath;
            }
        }
    }
}

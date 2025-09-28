using Application.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using PremiersoftChallenge.BuildingBlocks.Results;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Http
{
    internal sealed class ApiCall : IApiCall
    {
        private readonly ILoggedContext _loggedContext;

        public ApiCall(ILoggedContext loggedContext)
        {
            _loggedContext = loggedContext;
        }

        public async Task<Result<T>> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
        {
            var client = new HttpClient();

            using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _loggedContext.Token);

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync(cancellationToken);
                T? result = JsonSerializer.Deserialize<T>(data, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return Result.Success(result!);
            }

            var content = await response.Content.ReadAsStringAsync();
            var apiError = JsonSerializer.Deserialize<ApiErrorResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var error = new Error(
                code: apiError?.Title ?? "SERVER_ERROR",
                description: apiError?.Detail ?? "Error while sending the request.",
                type: MapStatusToErrorType(apiError?.Status ?? 500));

            return Result.Failure<T>(error);
        }

        private static ErrorType MapStatusToErrorType(int statusCode) =>
            statusCode switch
            {
                StatusCodes.Status400BadRequest => ErrorType.Validation,
                StatusCodes.Status404NotFound => ErrorType.NotFound,
                StatusCodes.Status409Conflict => ErrorType.Conflict,
                _ => ErrorType.Problem
            };

        public async Task<Result<TResponse>> PostAsync<TResponse, TRequest>(string endpoint, TRequest body, CancellationToken cancellationToken = default)
        {
            var client = new HttpClient();

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _loggedContext.Token);

            request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync(cancellationToken);

                TResponse? result = JsonSerializer.Deserialize<TResponse>(data, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return Result.Success(result!);
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiError = JsonSerializer.Deserialize<ApiErrorResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var error = new Error(
                code: apiError?.Title ?? "SERVER_ERROR",
                description: apiError?.Detail ?? "Error while sending the request.",
                type: MapStatusToErrorType(apiError?.Status ?? (int)response.StatusCode));

            return Result.Failure<TResponse>(error);
        }

        public async Task<Result> PostAsync<TRequest>(string endpoint, TRequest body, CancellationToken cancellationToken = default)
        {
            var client = new HttpClient();

            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _loggedContext.Token);

            request.Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return Result.Success();
            }

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var apiError = JsonSerializer.Deserialize<ApiErrorResponse>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var error = new Error(
                code: apiError?.Title ?? "SERVER_ERROR",
                description: apiError?.Detail ?? "Error while sending the request.",
                type: MapStatusToErrorType(apiError?.Status ?? (int)response.StatusCode));

            return Result.Failure(error);
        }
    }
}

﻿using PremiersoftChallenge.BuildingBlocks.Results;

namespace Api.Extensions
{
    public static class CustomResults
    {
        public static IResult Problem(Result result)
        {
            if (result.IsSuccess)
            {
                throw new InvalidOperationException();
            }

            return Results.Problem(
                title: GetTitle(result.Error),
                detail: GetDetail(result.Error),
                type: GetType(result.Error.Type),
                statusCode: GetStatusCode(result.Error.Type),
                extensions: GetErrors(result));

            static string GetTitle(Error error) =>
                error.Type switch
                {
                    ErrorType.Validation => error.Code,
                    ErrorType.Problem => error.Code,
                    ErrorType.NotFound => error.Code,
                    ErrorType.Conflict => error.Code,
                    ErrorType.Unauthorized => error.Code,
                    _ => "Server failure"
                };

            static string GetDetail(Error error) =>
                error.Type switch
                {
                    ErrorType.Validation => error.Description,
                    ErrorType.Problem => error.Description,
                    ErrorType.NotFound => error.Description,
                    ErrorType.Conflict => error.Description,
                    ErrorType.Unauthorized => error.Description,
                    _ => "Ocorreu um erro inesperado."
                };

            static string GetType(ErrorType errorType) =>
                errorType switch
                {
                    ErrorType.Validation => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                    ErrorType.Problem => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.1",
                    ErrorType.NotFound => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.4",
                    ErrorType.Conflict => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.5.8",
                    ErrorType.Unauthorized => "https://datatracker.ietf.org/doc/html/rfc7235#section-3.1",
                    _ => "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1"
                };

            static int GetStatusCode(ErrorType errorType) =>
                errorType switch
                {
                    ErrorType.Validation or ErrorType.Problem => StatusCodes.Status400BadRequest,
                    ErrorType.NotFound => StatusCodes.Status404NotFound,
                    ErrorType.Conflict => StatusCodes.Status409Conflict,
                    ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                    _ => StatusCodes.Status500InternalServerError
                };

            static Dictionary<string, object?>? GetErrors(Result result)
            {
                if (result.Error is not ValidationError validationError)
                {
                    return null;
                }

                return new Dictionary<string, object?>
                {
                    { "errors", validationError.Errors }
                };
            }
        }
    }
}

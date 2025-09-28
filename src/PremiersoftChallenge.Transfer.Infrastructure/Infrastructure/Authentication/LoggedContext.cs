using Application.Authentication;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Authentication
{
    internal sealed class LoggedContext : ILoggedContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggedContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid Id
        {
            get
            {
                return _httpContextAccessor
                    .HttpContext?
                    .User
                    .GetAccountId() ?? throw new Exception("Failed to fetch account id!");
            }
        }

        public string? Token
        {
            get
            {
                var authorizationHeader = _httpContextAccessor
                    .HttpContext?
                    .Request
                    .Headers
                    .Authorization
                    .FirstOrDefault();

                if (string.IsNullOrEmpty(authorizationHeader))
                {
                    return null;
                }

                return authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
                    ? authorizationHeader["Bearer ".Length..].Trim()
                    : authorizationHeader;
            }
        }
    }
}

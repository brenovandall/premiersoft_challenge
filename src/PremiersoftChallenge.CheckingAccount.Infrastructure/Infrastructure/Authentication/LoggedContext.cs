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

        public Guid Id =>
            _httpContextAccessor
            .HttpContext?
            .User
            .GetAccountId() ?? throw new Exception("Failed to fetch account id!");
    }
}

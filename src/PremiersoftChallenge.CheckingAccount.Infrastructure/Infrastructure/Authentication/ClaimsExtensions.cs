using System.Security.Claims;

namespace Infrastructure.Authentication
{
    internal static class ClaimsExtensions
    {
        public static Guid GetAccountId(this ClaimsPrincipal claims)
        {
            var id = claims?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(id, out Guid parsedId) ? parsedId : throw new Exception("Claim not found!");
        }
    }
}

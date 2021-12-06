using System.Security.Claims;

namespace ProjectBank.Client.Extensions;

public static class ClaimExtensions
{
    public static string? GetClaim(this IEnumerable<Claim> claims, string type)
    {
        return claims.FirstOrDefault(c => c.Type.Equals(type))?.Value;
    }
}
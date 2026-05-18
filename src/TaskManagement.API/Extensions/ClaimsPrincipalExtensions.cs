using System.Security.Claims;

namespace TaskManagement.API.Extensions;

internal static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        // Support common JWT/user-jwts claim names for user id.
        var candidateValues = user.Claims
            .Where(c =>
                c.Type == ClaimTypes.NameIdentifier ||
                c.Type == "sub" ||
                c.Type == "nameid" ||
                c.Type == "oid" ||
                c.Type == "uid" ||
                c.Type == "userId")
            .Select(c => c.Value);

        foreach (var candidate in candidateValues)
        {
            if (Guid.TryParse(candidate, out var parsedUserId))
            {
                return parsedUserId;
            }
        }

        throw new InvalidOperationException("The current user does not have a valid GUID user id claim.");
    }
}

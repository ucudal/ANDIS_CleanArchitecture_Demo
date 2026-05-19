using System.Security.Claims;

namespace TaskManagement.API.Extensions;

/// <summary>
/// ClaimsPrincipalExtensions provides extension methods for extracting user information from claims.
///
/// Role in Clean Architecture:
/// - Part of the UI Layer (API layer)
/// - Extension methods: Provides convenient helpers for claim extraction
/// - Encapsulates claim navigation logic
/// - Reduces repetition in controllers and services
///
/// Claims-Based Authentication:
/// - ClaimsPrincipal: Represents authenticated user with their claims
/// - Claims: Individual pieces of information about the user
/// - JWT tokens contain claims: user ID, email, roles, etc.
/// - Extensions simplify extracting specific claims
///
/// Design Benefits:
/// - Single source of truth for claim extraction logic
/// - Consistent claim handling across application
/// - Type-safe claim access
/// - Easy to modify claim structure
/// - Encapsulates security-related logic
///
/// Common Extensions:
/// - GetUserId(): Extract user ID from claims
/// - GetEmail(): Extract email from claims
/// - GetRoles(): Extract user roles from claims
///
/// Security Considerations:
/// - Only extract claims already present in token
/// - No modifications to claims (read-only)
/// - Validate token on request entry (done by authentication middleware)
/// - Extensions assume user is already authenticated
/// </summary>

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

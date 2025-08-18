using Domain.DTOs.Commom;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MockExams.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    private static string? GetClaim(this ClaimsPrincipal user, string claimType) =>
        user?.FindFirst(claimType)?.Value?.Trim();

    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var sub = user.GetClaim(ClaimTypes.NameIdentifier);
        return Guid.TryParse(sub, out var id) ? id : Guid.Empty;
    }

    public static string GetAppSlug(this ClaimsPrincipal user)
        => user.GetClaim("appSlug") ?? string.Empty;

    public static string GetPegasusRole(this ClaimsPrincipal user)
        => user.GetClaim("pegasusRole") ?? string.Empty;

    public static string GetAppRole(this ClaimsPrincipal user)
        => user.GetClaim("appRole") ?? string.Empty;

    public static string GetEmail(this ClaimsPrincipal user)
        => user.GetClaim(ClaimTypes.Email) ?? string.Empty;

    public static string GetUserName(this ClaimsPrincipal user)
        => user.GetClaim(ClaimTypes.Name)
        ?? user.GetClaim(JwtRegisteredClaimNames.UniqueName)
        ?? string.Empty;

    public static UserDto GetUser(this ClaimsPrincipal user)
    {
        return new UserDto
        {
            Id = GetUserId(user),
            Name = GetUserName(user),
            Email = GetEmail(user),
            AppRole = GetAppRole(user),
            AppSlug = GetAppSlug(user),
            PegasusRole = GetPegasusRole(user)
        };
    }

    public static bool IsOwnerOrAdmin(this ClaimsPrincipal user, Guid userId, string appSlug)
    {
        return user.GetUserId() == userId
            || user.IsAppAdmin(appSlug)
            || user.IsPegasusAdmin();
    }

    public static bool IsPegasusAdmin(this ClaimsPrincipal user)
    {
        return user.GetPegasusRole().Equals("admin", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsAppAdmin(this ClaimsPrincipal user, string appSlug)
    {
        if (user.IsPegasusAdmin())
            return true;

        var isAdmin = user.GetAppRole().Equals("admin", StringComparison.OrdinalIgnoreCase);

        if (!isAdmin)
            return false;

        return user.GetAppSlug().Equals(appSlug, StringComparison.OrdinalIgnoreCase);
    }
}

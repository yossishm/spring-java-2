// <copyright file="EnhancedAuthTestController.cs" company="SpringJavaEquivalent">
// Copyright (c) 2024. All rights reserved.
// </copyright>

namespace SpringJavaEquivalent.Controllers;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// Enhanced Authentication Test Controller demonstrating advanced authorization levels.
/// Equivalent to Spring's EnhancedAuthTestController with comprehensive security testing.
/// </summary>
[ApiController]
[Route("api/v1/enhanced-test")]
[Tags("Enhanced Authorization Test")]
public class EnhancedAuthTestController : ControllerBase
{
    private const string UnknownValue = "Unknown";

    private static readonly string[] NonePermissions = { "NONE" };
    private static readonly string[] BasicPermissions = { "BASIC_ACCESS" };
    private static readonly string[] UserPermissions = { "USER_ACCESS", "READ_DATA" };
    private static readonly string[] AdminPermissions = { "ADMIN_ACCESS", "WRITE_DATA", "DELETE_DATA" };
    private static readonly string[] ManagementPermissions = { "MANAGEMENT_ACCESS", "APPROVE_REQUESTS" };
    private static readonly string[] IdentityProviderPermissions = { "IDENTITY_PROVIDER_ACCESS" };
    private static readonly string[] HighAuthPermissions = { "HIGH_AUTH_ACCESS" };
    private static readonly string[] NoneRoles = { "NONE" };
    private const string PermissionClaimType = "permission";

    public EnhancedAuthTestController()
    {
    }

    /// <summary>
    /// Level 0: Public endpoint - no authentication required
    /// </summary>
    [HttpGet("public")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult PublicEndpoint()
    {
        return Ok(new
        {
            level = 0,
            message = "Public endpoint - no authentication required",
            timestamp = DateTime.UtcNow,
            user = UnknownValue,
            roles = NoneRoles,
            permissions = NonePermissions,
        });
    }

    /// <summary>
    /// Level 1: Basic authentication required
    /// </summary>
    [HttpGet("authenticated")]
    [Authorize]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult AuthenticatedEndpoint()
    {
        var user = this.User.Identity?.Name ?? UnknownValue;
        var roles = this.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToArray();

        return Ok(new
        {
            level = 1,
            message = "Authenticated endpoint - basic authentication required",
            timestamp = DateTime.UtcNow,
            user,
            roles,
            permissions = BasicPermissions,
        });
    }

    /// <summary>
    /// Level 2: Role-based authorization - USER role required
    /// </summary>
    [HttpGet("user-role")]
    [Authorize(Roles = "USER")]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult UserRoleEndpoint()
    {
        var user = this.User.Identity?.Name ?? UnknownValue;
        var roles = this.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToArray();

        return Ok(new
        {
            level = 2,
            message = "User role endpoint - USER role required",
            timestamp = DateTime.UtcNow,
            user,
            roles,
            permissions = UserPermissions,
        });
    }

    /// <summary>
    /// Level 3: Role-based authorization - ADMIN role required
    /// </summary>
    [HttpGet("admin-role")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult AdminRoleEndpoint()
    {
        var user = this.User.Identity?.Name ?? UnknownValue;
        var roles = this.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToArray();

        return Ok(new
        {
            level = 3,
            message = "Admin role endpoint - ADMIN role required",
            timestamp = DateTime.UtcNow,
            user,
            roles,
            permissions = AdminPermissions,
        });
    }

    /// <summary>
    /// Level 4: Permission-based authorization - CACHE_READ permission required
    /// </summary>
    [HttpGet("cache-read")]
    [Authorize(Policy = "CacheReadPolicy")]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult CacheReadEndpoint()
    {
        var user = this.User.Identity?.Name ?? UnknownValue;
        var roles = this.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToArray();
        var permissions = this.User.Claims
            .Where(c => c.Type == PermissionClaimType)
            .Select(c => c.Value)
            .ToArray();

        return Ok(new
        {
            level = 4,
            message = "Cache read endpoint - CACHE_READ permission required",
            timestamp = DateTime.UtcNow,
            user,
            roles,
            permissions,
        });
    }

    /// <summary>
    /// Level 5: Permission-based authorization - CACHE_WRITE permission required
    /// </summary>
    [HttpGet("cache-write")]
    [Authorize(Policy = "CacheWritePolicy")]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult CacheWriteEndpoint()
    {
        var user = this.User.Identity?.Name ?? UnknownValue;
        var roles = this.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToArray();
        var permissions = this.User.Claims
            .Where(c => c.Type == PermissionClaimType)
            .Select(c => c.Value)
            .ToArray();

        return Ok(new
        {
            level = 5,
            message = "Cache write endpoint - CACHE_WRITE permission required",
            timestamp = DateTime.UtcNow,
            user,
            roles,
            permissions,
        });
    }

    /// <summary>
    /// Level 6: Multiple role authorization - ADMIN or MANAGER role required
    /// </summary>
    [HttpGet("admin-or-manager")]
    [Authorize(Roles = "ADMIN,MANAGER")]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult AdminOrManagerEndpoint()
    {
        var user = this.User.Identity?.Name ?? UnknownValue;
        var roles = this.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToArray();

        return Ok(new
        {
            level = 6,
            message = "Admin or Manager endpoint - ADMIN or MANAGER role required",
            timestamp = DateTime.UtcNow,
            user,
            roles,
            permissions = ManagementPermissions,
        });
    }

    /// <summary>
    /// Level 7: Complex authorization - ADMIN role AND CACHE_WRITE permission required
    /// </summary>
    [HttpGet("admin-cache-write")]
    [Authorize(Roles = "ADMIN", Policy = "CacheWritePolicy")]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult AdminCacheWriteEndpoint()
    {
        var user = this.User.Identity?.Name ?? UnknownValue;
        var roles = this.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToArray();
        var permissions = this.User.Claims
            .Where(c => c.Type == PermissionClaimType)
            .Select(c => c.Value)
            .ToArray();

        return Ok(new
        {
            level = 7,
            message = "Admin cache write endpoint - ADMIN role AND CACHE_WRITE permission required",
            timestamp = DateTime.UtcNow,
            user,
            roles,
            permissions,
        });
    }

    /// <summary>
    /// Level 8: Custom authorization - Identity Provider requirement
    /// </summary>
    [HttpGet("identity-provider")]
    [Authorize(Policy = "IdentityProviderPolicy")]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult IdentityProviderEndpoint()
    {
        var user = this.User.Identity?.Name ?? UnknownValue;
        var roles = this.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToArray();
        var identityProvider = this.User.Claims
            .FirstOrDefault(c => c.Type == "identity_provider")?.Value ?? UnknownValue;

        return Ok(new
        {
            level = 8,
            message = "Identity provider endpoint - specific identity provider required",
            timestamp = DateTime.UtcNow,
            user,
            roles,
            identityProvider,
            permissions = IdentityProviderPermissions,
        });
    }

    /// <summary>
    /// Level 9: Auth Level requirement
    /// </summary>
    [HttpGet("auth-level")]
    [Authorize(Policy = "AuthLevelPolicy")]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult AuthLevelEndpoint()
    {
        var user = this.User.Identity?.Name ?? UnknownValue;
        var roles = this.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToArray();
        var authLevel = this.User.Claims
            .FirstOrDefault(c => c.Type == "auth_level")?.Value ?? UnknownValue;

        return Ok(new
        {
            level = 9,
            message = "Auth level endpoint - specific authentication level required",
            timestamp = DateTime.UtcNow,
            user,
            roles,
            authLevel,
            permissions = HighAuthPermissions,
        });
    }

    /// <summary>
    /// Level 10: Maximum security - All requirements
    /// </summary>
    [HttpGet("maximum-security")]
    [Authorize(Roles = "ADMIN", Policy = "CacheWritePolicy,IdentityProviderPolicy,AuthLevelPolicy")]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult MaximumSecurityEndpoint()
    {
        var user = this.User.Identity?.Name ?? UnknownValue;
        var roles = this.User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToArray();
        var permissions = this.User.Claims
            .Where(c => c.Type == PermissionClaimType)
            .Select(c => c.Value)
            .ToArray();
        var identityProvider = this.User.Claims
            .FirstOrDefault(c => c.Type == "identity_provider")?.Value ?? UnknownValue;
        var authLevel = this.User.Claims
            .FirstOrDefault(c => c.Type == "auth_level")?.Value ?? UnknownValue;

        return Ok(new
        {
            level = 10,
            message = "Maximum security endpoint - all authorization requirements",
            timestamp = DateTime.UtcNow,
            user,
            roles,
            permissions,
            identityProvider,
            authLevel,
        });
    }
}
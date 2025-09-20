using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace SpringJavaEquivalent.Controllers;

/// <summary>
/// Enhanced Authentication Test Controller demonstrating advanced authorization levels.
/// Equivalent to Spring's EnhancedAuthTestController with comprehensive security testing.
/// </summary>
[ApiController]
[Route("api/v1/enhanced-test")]
[Tags("Enhanced Authorization Test")]
public class EnhancedAuthTestController : ControllerBase
{
    private readonly ILogger<EnhancedAuthTestController> _logger;

    public EnhancedAuthTestController(ILogger<EnhancedAuthTestController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Level 0: Public endpoint - no authentication required
    /// </summary>
    [HttpGet("public")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult PublicEndpoint()
    {
        var response = new
        {
            message = "This is a public endpoint (Level 0)",
            level = 0,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        return Ok(response);
    }

    /// <summary>
    /// Level 1: Basic authentication required
    /// </summary>
    [HttpGet("authenticated")]
    [Authorize]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(401)]
    public IActionResult AuthenticatedEndpoint()
    {
        var username = User.Identity?.Name ?? "Unknown";
        var response = new
        {
            message = "This is an authenticated endpoint (Level 1)",
            level = 1,
            username,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        return Ok(response);
    }

    /// <summary>
    /// Level 2: Role-based authorization
    /// </summary>
    [HttpGet("role-based")]
    [Authorize(Roles = "USER")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public IActionResult RoleBasedEndpoint()
    {
        var username = User.Identity?.Name ?? "Unknown";
        var roles = User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        var response = new
        {
            message = "This is a role-based endpoint (Level 2)",
            level = 2,
            username,
            roles,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        return Ok(response);
    }

    /// <summary>
    /// Level 3: Admin role authorization
    /// </summary>
    [HttpGet("admin-only")]
    [Authorize(Roles = "ADMIN")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public IActionResult AdminOnlyEndpoint()
    {
        var username = User.Identity?.Name ?? "Unknown";
        var response = new
        {
            message = "This is an admin-only endpoint (Level 3)",
            level = 3,
            username,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        return Ok(response);
    }

    /// <summary>
    /// Level 4: Permission-based authorization
    /// </summary>
    [HttpGet("permission-based")]
    [Authorize(Policy = "RequireCacheReadPermission")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public IActionResult PermissionBasedEndpoint()
    {
        var username = User.Identity?.Name ?? "Unknown";
        var permissions = User.Claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToList();

        var response = new
        {
            message = "This is a permission-based endpoint (Level 4)",
            level = 4,
            username,
            permissions,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        return Ok(response);
    }

    /// <summary>
    /// Level 5: Authentication Level-based authorization (AAL)
    /// </summary>
    [HttpGet("aal2-required")]
    [Authorize(Policy = "RequireAAL2OrHigher")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public IActionResult Aal2RequiredEndpoint()
    {
        var username = User.Identity?.Name ?? "Unknown";
        var authLevels = User.Claims
            .Where(c => c.Type == "auth_level")
            .Select(c => c.Value)
            .ToList();

        var response = new
        {
            message = "This is an AAL2+ endpoint (Level 5)",
            level = 5,
            username,
            authLevels,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        return Ok(response);
    }

    /// <summary>
    /// Level 6: Identity Provider-based authorization
    /// </summary>
    [HttpGet("enterprise-only")]
    [Authorize(Policy = "RequireEnterpriseIdp")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public IActionResult EnterpriseOnlyEndpoint()
    {
        var username = User.Identity?.Name ?? "Unknown";
        var idps = User.Claims
            .Where(c => c.Type == "idp")
            .Select(c => c.Value)
            .ToList();

        var response = new
        {
            message = "This is an enterprise-only endpoint (Level 6)",
            level = 6,
            username,
            identityProviders = idps,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        return Ok(response);
    }

    /// <summary>
    /// Level 7: Complex multi-factor authorization
    /// </summary>
    [HttpGet("multi-factor")]
    [Authorize(Policy = "RequireMultiFactorAuth")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public IActionResult MultiFactorEndpoint()
    {
        var username = User.Identity?.Name ?? "Unknown";
        var roles = User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        var permissions = User.Claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToList();

        var authLevels = User.Claims
            .Where(c => c.Type == "auth_level")
            .Select(c => c.Value)
            .ToList();

        var idps = User.Claims
            .Where(c => c.Type == "idp")
            .Select(c => c.Value)
            .ToList();

        var response = new
        {
            message = "This is a multi-factor endpoint (Level 7)",
            level = 7,
            username,
            roles,
            permissions,
            authLevels,
            identityProviders = idps,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };
        return Ok(response);
    }

    /// <summary>
    /// Get current user's authorization context
    /// </summary>
    [HttpGet("context")]
    [Authorize]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(401)]
    public IActionResult GetAuthorizationContext()
    {
        var username = User.Identity?.Name ?? "Unknown";
        var roles = User.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        var permissions = User.Claims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .ToList();

        var authLevels = User.Claims
            .Where(c => c.Type == "auth_level")
            .Select(c => c.Value)
            .ToList();

        var idps = User.Claims
            .Where(c => c.Type == "idp")
            .Select(c => c.Value)
            .ToList();

        var allClaims = User.Claims
            .ToDictionary(c => c.Type, c => c.Value);

        var response = new
        {
            username,
            roles,
            permissions,
            authLevels,
            identityProviders = idps,
            allClaims,
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        return Ok(response);
    }
}

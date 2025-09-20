using Microsoft.AspNetCore.Mvc;
using SpringJavaEquivalent.Services;
using System.ComponentModel.DataAnnotations;

namespace SpringJavaEquivalent.Controllers;

/// <summary>
/// Controller for generating JWT tokens for testing purposes.
/// Equivalent to Spring's TokenController with comprehensive functionality.
/// </summary>
[ApiController]
[Route("api/v1/auth")]
[Tags("Authentication")]
public class TokenController : ControllerBase
{
    private readonly JwtService _jwtService;
    private readonly ILogger<TokenController> _logger;

    public TokenController(JwtService jwtService, ILogger<TokenController> logger)
    {
        _jwtService = jwtService;
        _logger = logger;
    }

    /// <summary>
    /// Generate a JWT token with specified roles and permissions
    /// </summary>
    [HttpPost("token")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    public IActionResult GenerateToken(
        [FromQuery, Required] string username,
        [FromQuery] string? roles = null,
        [FromQuery] string? permissions = null)
    {
        // Parse roles and permissions from comma-separated strings
        var rolesList = string.IsNullOrEmpty(roles) 
            ? new List<string> { "USER" } 
            : roles.Split(',').Select(r => r.Trim()).ToList();
        
        var permissionsList = string.IsNullOrEmpty(permissions) 
            ? new List<string> { "CACHE_READ" } 
            : permissions.Split(',').Select(p => p.Trim()).ToList();

        var token = _jwtService.GenerateToken(username, rolesList, permissionsList);

        var response = new
        {
            token,
            username,
            roles = rolesList,
            permissions = permissionsList,
            expiresIn = "24 hours",
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        return Ok(response);
    }

    /// <summary>
    /// Generate predefined token types for testing
    /// </summary>
    [HttpGet("token/{type}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    public IActionResult GeneratePredefinedToken(string type)
    {
        string username;
        List<string> roles;
        List<string> permissions;
        string authLevel;
        string idp;

        switch (type.ToLower())
        {
            case "admin":
                username = "admin";
                roles = new List<string> { "ADMIN", "USER" };
                permissions = new List<string> { "CACHE_READ", "CACHE_WRITE", "CACHE_DELETE", "CACHE_ADMIN" };
                authLevel = "AAL3"; // High security for admin
                idp = "enterprise-ldap";
                break;
            case "user":
                username = "user";
                roles = new List<string> { "USER" };
                permissions = new List<string> { "CACHE_READ" };
                authLevel = "AAL1"; // Basic auth for regular user
                idp = "local";
                break;
            case "cache-admin":
                username = "cache-admin";
                roles = new List<string> { "USER" };
                permissions = new List<string> { "CACHE_READ", "CACHE_WRITE", "CACHE_DELETE", "CACHE_ADMIN" };
                authLevel = "AAL2"; // MFA for cache admin
                idp = "azure-ad";
                break;
            case "cache-writer":
                username = "cache-writer";
                roles = new List<string> { "USER" };
                permissions = new List<string> { "CACHE_READ", "CACHE_WRITE" };
                authLevel = "AAL2"; // MFA for write operations
                idp = "okta";
                break;
            case "cache-reader":
                username = "cache-reader";
                roles = new List<string> { "USER" };
                permissions = new List<string> { "CACHE_READ" };
                authLevel = "AAL1"; // Basic auth for read-only
                idp = "local";
                break;
            case "aal1-user":
                username = "aal1-user";
                roles = new List<string> { "USER" };
                permissions = new List<string> { "CACHE_READ" };
                authLevel = "AAL1";
                idp = "local";
                break;
            case "aal2-user":
                username = "aal2-user";
                roles = new List<string> { "USER" };
                permissions = new List<string> { "CACHE_READ", "CACHE_WRITE" };
                authLevel = "AAL2";
                idp = "azure-ad";
                break;
            case "aal3-user":
                username = "aal3-user";
                roles = new List<string> { "ADMIN" };
                permissions = new List<string> { "CACHE_READ", "CACHE_WRITE", "CACHE_DELETE", "CACHE_ADMIN" };
                authLevel = "AAL3";
                idp = "enterprise-ldap";
                break;
            default:
                return BadRequest(new { error = "Invalid token type" });
        }

        var token = _jwtService.GenerateToken(username, roles, permissions, authLevel, idp);

        var response = new
        {
            token,
            username,
            roles,
            permissions,
            auth_level = authLevel,
            idp,
            type,
            expiresIn = "24 hours",
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        };

        return Ok(response);
    }

    /// <summary>
    /// Validate a JWT token
    /// </summary>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult ValidateToken([Required] string token)
    {
        var isValid = _jwtService.ValidateToken(token);
        
        if (isValid)
        {
            var response = new
            {
                valid = isValid,
                username = _jwtService.ExtractUsername(token),
                roles = _jwtService.ExtractRoles(token),
                permissions = _jwtService.ExtractPermissions(token),
                auth_level = _jwtService.ExtractAuthLevel(token),
                idp = _jwtService.ExtractIdentityProvider(token),
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
            return Ok(response);
        }
        else
        {
            var response = new
            {
                valid = isValid,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
            return Ok(response);
        }
    }
}

// <copyright file="TokenController.cs" company="SpringJavaEquivalent">
// Copyright (c) 2024. All rights reserved.
// </copyright>

namespace SpringJavaEquivalent.Controllers;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SpringJavaEquivalent.Services;

/// <summary>
/// Controller for generating JWT tokens for testing purposes.
/// Equivalent to Spring's TokenController with comprehensive functionality.
/// </summary>
[ApiController]
[Route("api/v1/auth")]
[Tags("Authentication")]
public class TokenController : ControllerBase
{
    private readonly JwtService jwtService;
    private const string CacheReadPermission = "CACHE_READ";
    private const string CacheWritePermission = "CACHE_WRITE";
    private const string DefaultRole = "USER";

    private static readonly string[] AdminRoles = { "ADMIN", "USER" };
    private static readonly string[] AdminPermissions = { CacheReadPermission, CacheWritePermission, "ADMIN_ACCESS" };
    private static readonly string[] UserRoles = { DefaultRole };
    private static readonly string[] UserPermissions = { CacheReadPermission };
    private static readonly string[] ReadOnlyRoles = { "READONLY" };
    private static readonly string[] ReadOnlyPermissions = { CacheReadPermission };
    private static readonly string[] WriterRoles = { "WRITER" };
    private static readonly string[] WriterPermissions = { CacheReadPermission, CacheWritePermission };

    public TokenController(JwtService jwtService)
    {
        this.jwtService = jwtService;
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
            ? new List<string> { DefaultRole }
            : roles.Split(',').Select(r => r.Trim()).ToList();

        var permissionsList = string.IsNullOrEmpty(permissions)
            ? new List<string> { CacheReadPermission }
            : permissions.Split(',').Select(p => p.Trim()).ToList();

        var token = this.jwtService.GenerateToken(username, rolesList, permissionsList);

        var response = new
        {
            token,
            username,
            roles = rolesList,
            permissions = permissionsList,
            expiresAt = DateTime.UtcNow.AddHours(1),
        };

        return Ok(response);
    }

    /// <summary>
    /// Generate a predefined token based on type
    /// </summary>
    [HttpGet("predefined/{type}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    public IActionResult GeneratePredefinedToken([FromRoute] string type)
    {
        ArgumentNullException.ThrowIfNull(type);
        
        var upperType = type.ToUpperInvariant();
        return upperType switch
        {
            "ADMIN" => this.GenerateAdminToken(),
            "USER" => this.GenerateUserToken(),
            "READONLY" => this.GenerateReadOnlyToken(),
            "WRITE" => this.GenerateWriteToken(),
            _ => BadRequest($"Unknown token type: {type}"),
        };
    }

    /// <summary>
    /// Generate an admin token with full permissions
    /// </summary>
    [HttpGet("admin")]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult GenerateAdminToken()
    {
        var token = this.jwtService.GenerateToken(
            "admin",
            new List<string> { "ADMIN", "USER" },
            new List<string> { CacheReadPermission, CacheWritePermission, "ADMIN_ACCESS" });

        return Ok(new
        {
            token,
            username = "admin",
            roles = AdminRoles,
            permissions = AdminPermissions,
            expiresAt = DateTime.UtcNow.AddHours(1),
        });
    }

    /// <summary>
    /// Generate a regular user token
    /// </summary>
    [HttpGet("user")]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult GenerateUserToken()
    {
        var token = this.jwtService.GenerateToken(
            "user",
            new List<string> { DefaultRole },
            new List<string> { CacheReadPermission });

        return Ok(new
        {
            token,
            username = "user",
            roles = UserRoles,
            permissions = UserPermissions,
            expiresAt = DateTime.UtcNow.AddHours(1),
        });
    }

    /// <summary>
    /// Generate a read-only token
    /// </summary>
    [HttpGet("readonly")]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult GenerateReadOnlyToken()
    {
        var token = this.jwtService.GenerateToken(
            "readonly",
            new List<string> { "READONLY" },
            new List<string> { CacheReadPermission });

        return Ok(new
        {
            token,
            username = "readonly",
            roles = ReadOnlyRoles,
            permissions = ReadOnlyPermissions,
            expiresAt = DateTime.UtcNow.AddHours(1),
        });
    }

    /// <summary>
    /// Generate a write token
    /// </summary>
    [HttpGet("write")]
    [ProducesResponseType(typeof(object), 200)]
    public IActionResult GenerateWriteToken()
    {
        var token = this.jwtService.GenerateToken(
            "writer",
            new List<string> { "WRITER" },
            new List<string> { CacheReadPermission, CacheWritePermission });

        return Ok(new
        {
            token,
            username = "writer",
            roles = WriterRoles,
            permissions = WriterPermissions,
            expiresAt = DateTime.UtcNow.AddHours(1),
        });
    }

    /// <summary>
    /// Validate a JWT token
    /// </summary>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(400)]
    public IActionResult ValidateToken([FromBody] Dictionary<string, string> request)
    {
        ArgumentNullException.ThrowIfNull(request);
        
        if (!request.TryGetValue("token", out var token))
        {
            return BadRequest("Token is required");
        }

        try
        {
            var isValid = this.jwtService.ValidateToken(token);
            var username = JwtService.ExtractUsername(token);
            var roles = JwtService.ExtractRoles(token);
            var permissions = JwtService.ExtractPermissions(token);

            return Ok(new
            {
                valid = isValid,
                username,
                roles,
                permissions,
                validatedAt = DateTime.UtcNow,
            });
        }
        catch (Exception ex)
        {
            return Ok(new
            {
                valid = false,
                error = ex.Message,
                validatedAt = DateTime.UtcNow,
            });
        }
    }
}
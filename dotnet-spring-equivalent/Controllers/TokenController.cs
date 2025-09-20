// <copyright file="TokenController.cs" company="SpringJavaEquivalent">
// Copyright (c) 2024. All rights reserved.
// </copyright>

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
    private const string CacheReadPermission = "CACHE_READ";
    private const string CacheWritePermission = "CACHE_WRITE";
    private const string DefaultRole = "USER";

    public TokenController(JwtService jwtService)
    {
        this._jwtService = jwtService;
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

        var token = this._jwtService.GenerateToken(username, rolesList, permissionsList);

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
        var lowerType = type.ToLowerInvariant();
        return lowerType switch
        {
            "admin" => this.GenerateAdminToken(),
            "user" => this.GenerateUserToken(),
            "readonly" => this.GenerateReadOnlyToken(),
            "write" => this.GenerateWriteToken(),
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
        var token = this._jwtService.GenerateToken(
            "admin",
            new List<string> { "ADMIN", "USER" },
            new List<string> { CacheReadPermission, CacheWritePermission, "ADMIN_ACCESS" });

        return Ok(new
        {
            token,
            username = "admin",
            roles = new[] { "ADMIN", "USER" },
            permissions = new[] { CacheReadPermission, CacheWritePermission, "ADMIN_ACCESS" },
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
        var token = this._jwtService.GenerateToken(
            "user",
            new List<string> { DefaultRole },
            new List<string> { CacheReadPermission });

        return Ok(new
        {
            token,
            username = "user",
            roles = new[] { DefaultRole },
            permissions = new[] { CacheReadPermission },
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
        var token = this._jwtService.GenerateToken(
            "readonly",
            new List<string> { "READONLY" },
            new List<string> { CacheReadPermission });

        return Ok(new
        {
            token,
            username = "readonly",
            roles = new[] { "READONLY" },
            permissions = new[] { CacheReadPermission },
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
        var token = this._jwtService.GenerateToken(
            "writer",
            new List<string> { "WRITER" },
            new List<string> { CacheReadPermission, CacheWritePermission });

        return Ok(new
        {
            token,
            username = "writer",
            roles = new[] { "WRITER" },
            permissions = new[] { CacheReadPermission, CacheWritePermission },
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
        if (!request.TryGetValue("token", out var token))
        {
            return BadRequest("Token is required");
        }

        try
        {
            var isValid = this._jwtService.ValidateToken(token);
            var username = this._jwtService.ExtractUsername(token);
            var roles = this._jwtService.ExtractRoles(token);
            var permissions = this._jwtService.ExtractPermissions(token);

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
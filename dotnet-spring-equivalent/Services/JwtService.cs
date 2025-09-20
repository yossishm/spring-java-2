// <copyright file="JwtService.cs" company="SpringJavaEquivalent">
// Copyright (c) 2024. All rights reserved.
// </copyright>

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SpringJavaEquivalent.Services;

/// <summary>
/// JWT service for token generation, validation, and claims extraction.
/// Equivalent to Spring's JwtUtil with comprehensive functionality.
/// </summary>
public class JwtService
{
    private readonly string _secretKey;
    private readonly int _expirationHours;

    public JwtService(IConfiguration configuration)
    {
        this._secretKey = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT secret must be configured");
        this._expirationHours = int.Parse(configuration["Jwt:ExpirationHours"] ?? "24", System.Globalization.CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Generate JWT token with roles and permissions
    /// </summary>
    public string GenerateToken(string username, IReadOnlyList<string> roles, IReadOnlyList<string> permissions,
        string authLevel = "AAL1", string idp = "local")
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(this._secretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new("username", username),
            new("auth_level", authLevel),
            new("identity_provider", idp),
        };

        // Add role claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Add permission claims
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(this._expirationHours),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Validate JWT token
    /// </summary>
    public bool ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(this._secretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero,
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Extract username from JWT token
    /// </summary>
    public string ExtractUsername(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            return jsonToken.Claims.FirstOrDefault(x => x.Type == "username")?.Value ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Extract roles from JWT token
    /// </summary>
    public IReadOnlyList<string> ExtractRoles(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            return jsonToken.Claims
                .Where(x => x.Type == ClaimTypes.Role)
                .Select(x => x.Value)
                .ToList();
        }
        catch
        {
            return new List<string>();
        }
    }

    /// <summary>
    /// Extract permissions from JWT token
    /// </summary>
    public IReadOnlyList<string> ExtractPermissions(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            return jsonToken.Claims
                .Where(x => x.Type == "permission")
                .Select(x => x.Value)
                .ToList();
        }
        catch
        {
            return new List<string>();
        }
    }

    /// <summary>
    /// Extract authentication level from JWT token
    /// </summary>
    public string ExtractAuthLevel(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            return jsonToken.Claims.FirstOrDefault(x => x.Type == "auth_level")?.Value ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Extract identity provider from JWT token
    /// </summary>
    public string ExtractIdentityProvider(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            return jsonToken.Claims.FirstOrDefault(x => x.Type == "identity_provider")?.Value ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// Extract all claims from JWT token
    /// </summary>
    public Dictionary<string, string> ExtractAllClaims(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            return jsonToken.Claims.ToDictionary(x => x.Type, x => x.Value);
        }
        catch
        {
            return new Dictionary<string, string>();
        }
    }
}
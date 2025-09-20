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
    private readonly IConfiguration _configuration;
    private readonly string _secretKey;
    private readonly int _expirationHours;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
        _secretKey = _configuration["Jwt:Secret"] ?? "mySecretKeyForJWTTokenGenerationAndValidation123456789";
        _expirationHours = int.Parse(_configuration["Jwt:ExpirationHours"] ?? "24");
    }

    /// <summary>
    /// Generate JWT token with roles and permissions
    /// </summary>
    public string GenerateToken(string username, List<string> roles, List<string> permissions, 
        string authLevel = "AAL1", string idp = "local")
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new(ClaimTypes.NameIdentifier, username),
            new("auth_level", authLevel),
            new("idp", idp)
        };

        // Add roles as claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Add permissions as claims
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(_expirationHours),
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
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
            var key = Encoding.ASCII.GetBytes(_secretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Extract username from token
    /// </summary>
    public string? ExtractUsername(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            return jsonToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name)?.Value;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Extract roles from token
    /// </summary>
    public List<string> ExtractRoles(string token)
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
    /// Extract permissions from token
    /// </summary>
    public List<string> ExtractPermissions(string token)
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
    /// Extract authentication level from token
    /// </summary>
    public string ExtractAuthLevel(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            return jsonToken.Claims.FirstOrDefault(x => x.Type == "auth_level")?.Value ?? "AAL1";
        }
        catch
        {
            return "AAL1";
        }
    }

    /// <summary>
    /// Extract identity provider from token
    /// </summary>
    public string ExtractIdentityProvider(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            return jsonToken.Claims.FirstOrDefault(x => x.Type == "idp")?.Value ?? "local";
        }
        catch
        {
            return "local";
        }
    }

    /// <summary>
    /// Extract all claims from token
    /// </summary>
    public Dictionary<string, object> ExtractAllClaims(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jsonToken = tokenHandler.ReadJwtToken(token);
            
            var claims = new Dictionary<string, object>();
            foreach (var claim in jsonToken.Claims)
            {
                if (claims.ContainsKey(claim.Type))
                {
                    if (claims[claim.Type] is List<string> list)
                    {
                        list.Add(claim.Value);
                    }
                    else
                    {
                        claims[claim.Type] = new List<string> { claims[claim.Type].ToString()!, claim.Value };
                    }
                }
                else
                {
                    claims[claim.Type] = claim.Value;
                }
            }
            
            return claims;
        }
        catch
        {
            return new Dictionary<string, object>();
        }
    }
}

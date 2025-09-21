using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

// Create a simple JWT token with roles
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("test-secret-key-that-is-long-enough-for-hmac-sha256"));
var tokenHandler = new JwtSecurityTokenHandler();

var claims = new List<Claim>
{
    new(ClaimTypes.Name, "testuser"),
    new("username", "testuser"),
    new(ClaimTypes.Role, "USER"),
    new(ClaimTypes.Role, "ADMIN"),
};

var tokenDescriptor = new SecurityTokenDescriptor
{
    Subject = new ClaimsIdentity(claims),
    Expires = DateTime.UtcNow.AddHours(24),
    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature),
};

var token = tokenHandler.CreateToken(tokenDescriptor);
var tokenString = tokenHandler.WriteToken(token);

Console.WriteLine($"Generated token: {tokenString}");

// Now try to extract roles
var jsonToken = tokenHandler.ReadJwtToken(tokenString);

Console.WriteLine("All claims:");
foreach (var claim in jsonToken.Claims)
{
    Console.WriteLine($"  {claim.Type} = {claim.Value}");
}

Console.WriteLine($"ClaimTypes.Role = {ClaimTypes.Role}");

var roles = jsonToken.Claims
    .Where(x => x.Type == ClaimTypes.Role)
    .Select(x => x.Value)
    .ToList();

Console.WriteLine($"Extracted roles: {string.Join(", ", roles)}");
Console.WriteLine($"Role count: {roles.Count}");

using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace SpringJavaEquivalent.Controllers;

[ApiController]
[Route("api/jwt")]
public class VulnerableJWTController : ControllerBase
{
    private readonly ILogger<VulnerableJWTController> _logger;

    // Vulnerable: Using weak secret key (same as Spring version)
    private static readonly string WEAK_SECRET = "mySecretKey123";

    public VulnerableJWTController(ILogger<VulnerableJWTController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Create a vulnerable JWT token - equivalent to Spring's create endpoint
    /// </summary>
    [HttpPost("create")]
    public IActionResult CreateToken([FromBody] Dictionary<string, object> payload)
    {
        try
        {
            // Vulnerable: Simple base64 encoding (not real JWT) - same as Spring version
            var header = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"alg\":\"HS256\",\"typ\":\"JWT\"}"));
            var payloadStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload)));
            var signature = Convert.ToBase64String(Encoding.UTF8.GetBytes(WEAK_SECRET));

            var token = $"{header}.{payloadStr}.{signature}";
            return Ok(token);
        }
        catch (Exception e)
        {
            return BadRequest($"Error creating token: {e.Message}");
        }
    }

    /// <summary>
    /// Verify a JWT token - equivalent to Spring's verify endpoint
    /// </summary>
    [HttpPost("verify")]
    public IActionResult VerifyToken([FromBody] Dictionary<string, string> request)
    {
        try
        {
            var token = request.GetValueOrDefault("token", "");

            // Vulnerable: No proper validation - same as Spring version
            var response = new Dictionary<string, object>
            {
                ["valid"] = true,
                ["token"] = token,
                ["message"] = "Token accepted without proper validation"
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            var response = new Dictionary<string, object>
            {
                ["valid"] = false,
                ["error"] = e.Message
            };

            return Ok(response);
        }
    }

    /// <summary>
    /// Decode a JWT token - equivalent to Spring's decode endpoint
    /// </summary>
    [HttpGet("decode")]
    public IActionResult DecodeToken([FromQuery] string token)
    {
        try
        {
            // Vulnerable: Decoding without verification - same as Spring version
            var parts = token.Split('.');
            var response = new Dictionary<string, object>
            {
                ["decoded"] = true,
                ["header"] = parts.Length > 0 ? Encoding.UTF8.GetString(Convert.FromBase64String(parts[0])) : "",
                ["payload"] = parts.Length > 1 ? Encoding.UTF8.GetString(Convert.FromBase64String(parts[1])) : "",
                ["signature"] = parts.Length > 2 ? parts[2] : ""
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            var response = new Dictionary<string, object>
            {
                ["decoded"] = false,
                ["error"] = e.Message
            };

            return Ok(response);
        }
    }

    /// <summary>
    /// Verify token with any algorithm - equivalent to Spring's verify-any-algorithm endpoint
    /// </summary>
    [HttpPost("verify-any-algorithm")]
    public IActionResult VerifyAnyAlgorithm([FromBody] Dictionary<string, string> request)
    {
        try
        {
            var token = request.GetValueOrDefault("token", "");
            var algorithm = request.GetValueOrDefault("algorithm", ""); // Vulnerable: user-controlled algorithm

            // Vulnerable: Accepts any algorithm without validation - same as Spring version
            var response = new Dictionary<string, object>
            {
                ["valid"] = true,
                ["algorithm"] = algorithm,
                ["token"] = token,
                ["message"] = $"Algorithm accepted without validation: {algorithm}"
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            var response = new Dictionary<string, object>
            {
                ["valid"] = false,
                ["error"] = e.Message
            };

            return Ok(response);
        }
    }
}

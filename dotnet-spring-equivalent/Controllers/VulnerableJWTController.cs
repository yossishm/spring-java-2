// <copyright file="VulnerableJWTController.cs" company="SpringJavaEquivalent">
// Copyright (c) 2024. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace SpringJavaEquivalent.Controllers;

[ApiController]
[Route("api/jwt")]
public class VulnerableJwtController : ControllerBase
{
    // SECURITY WARNING: This is intentionally vulnerable for demonstration purposes
    // In production, use strong, randomly generated secrets from secure configuration
    private const string WeakSecret = "DEMO_WEAK_SECRET_DO_NOT_USE_IN_PRODUCTION";
    private const string TokenKey = "token";
    private const string AlgorithmKey = "algorithm";
    private const string ValidKey = "valid";
    private const string DecodedKey = "decoded";
    private const string HeaderKey = "header";
    private const string PayloadKey = "payload";
    private const string SignatureKey = "signature";
    private const string ErrorKey = "error";
    private const string MessageKey = "message";

    public VulnerableJwtController()
    {
    }

    /// <summary>
    /// Create a vulnerable JWT token - equivalent to Spring's create endpoint
    /// SECURITY WARNING: This endpoint is intentionally vulnerable for demonstration purposes
    /// </summary>
    [HttpPost("create")]
    public IActionResult CreateToken([FromBody] Dictionary<string, object> payload)
    {
        try
        {
            // Vulnerable: Simple base64 encoding (not real JWT) - same as Spring version
            var header = Convert.ToBase64String(Encoding.UTF8.GetBytes("{\"alg\":\"HS256\",\"typ\":\"JWT\"}"));
            var payloadStr = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload)));
            var signature = Convert.ToBase64String(Encoding.UTF8.GetBytes(WeakSecret));

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
            var token = request.GetValueOrDefault(TokenKey, string.Empty);

            // Vulnerable: No proper validation - same as Spring version
            var response = new Dictionary<string, object>
            {
                [ValidKey] = true,
                [TokenKey] = token,
                [MessageKey] = "Token accepted without proper validation",
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            var response = new Dictionary<string, object>
            {
                [ValidKey] = false,
                [ErrorKey] = e.Message,
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
                [DecodedKey] = true,
                [HeaderKey] = parts.Length > 0 ? Encoding.UTF8.GetString(Convert.FromBase64String(parts[0])) : string.Empty,
                [PayloadKey] = parts.Length > 1 ? Encoding.UTF8.GetString(Convert.FromBase64String(parts[1])) : string.Empty,
                [SignatureKey] = parts.Length > 2 ? parts[2] : string.Empty,
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            var response = new Dictionary<string, object>
            {
                [DecodedKey] = false,
                [ErrorKey] = e.Message,
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
            var token = request.GetValueOrDefault(TokenKey, string.Empty);
            var algorithm = request.GetValueOrDefault(AlgorithmKey, string.Empty); // Vulnerable: user-controlled algorithm

            // Vulnerable: Accepts any algorithm without validation - same as Spring version
            var response = new Dictionary<string, object>
            {
                [ValidKey] = true,
                [AlgorithmKey] = algorithm,
                [TokenKey] = token,
                [MessageKey] = $"Algorithm accepted without validation: {algorithm}",
            };

            return Ok(response);
        }
        catch (Exception e)
        {
            var response = new Dictionary<string, object>
            {
                [ValidKey] = false,
                [ErrorKey] = e.Message,
            };

            return Ok(response);
        }
    }
}
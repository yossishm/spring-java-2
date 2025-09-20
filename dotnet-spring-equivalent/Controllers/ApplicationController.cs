using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace SpringJavaEquivalent.Controllers;

[ApiController]
[Route("")]
[Tags("Cache Services")]
public class ApplicationController : ControllerBase
{
    private readonly ILogger<ApplicationController> _logger;

    public ApplicationController(ILogger<ApplicationController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Home endpoint - equivalent to Spring's root endpoint
    /// </summary>
    [HttpGet]
    public IActionResult Home()
    {
        // Equivalent to Spring's base64 encoding of "Authorization: Bearer"
        var jwsHeader = Convert.ToBase64String(Encoding.UTF8.GetBytes("Authorization: Bearer"));
        _logger.LogInformation("jws header base64 is: {JwsHeader}", jwsHeader);

        // Log all headers (equivalent to Spring's header iteration)
        foreach (var header in Request.Headers)
        {
            _logger.LogInformation("Header '{Key}' = {Value}", header.Key, header.Value);
        }

        return Ok("Hello Docker Yossi World");
    }

    /// <summary>
    /// Cache service - get object endpoint
    /// </summary>
    [HttpGet("api/v1/cacheServices/getObject")]
    [Authorize(Policy = "RequireCacheReadOrAdminPermission")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public IActionResult GetObject([FromQuery] string id)
    {
        _logger.LogInformation("Get: {Id} Called", id);
        return Ok("get");
    }

    /// <summary>
    /// Cache service - put object endpoint
    /// </summary>
    [HttpPut("api/v1/cacheServices/putObject")]
    [Authorize(Policy = "RequireCacheWriteOrAdminPermission")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public IActionResult PutObject([FromQuery] string id)
    {
        _logger.LogInformation("Put: {Id} Called", id);
        return Ok("put");
    }

    /// <summary>
    /// Cache service - delete object endpoint
    /// </summary>
    [HttpDelete("api/v1/cacheServices/deleteObject")]
    [Authorize(Policy = "RequireCacheDeleteOrAdminPermission")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(403)]
    public IActionResult DeleteObject([FromQuery] string id)
    {
        _logger.LogInformation("Delete: {Id} Called", id);
        return Ok("delete");
    }
}
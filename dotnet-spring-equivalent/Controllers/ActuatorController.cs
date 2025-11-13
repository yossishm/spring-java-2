// <copyright file="ActuatorController.cs" company="SpringJavaEquivalent">
// Copyright (c) 2024. All rights reserved.
// </copyright>

namespace SpringJavaEquivalent.Controllers;

using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

/// <summary>
/// Provides Actuator-style diagnostics endpoints aligning .NET with the Spring implementation.
/// </summary>
[ApiController]
[Route("actuator")]
[Tags("Diagnostics")]
[SuppressMessage("Major Code Smell", "S6960", Justification = "Actuator endpoints intentionally consolidated to mirror Spring Boot Actuator layout.")]
public class ActuatorController : ControllerBase
{
    private static readonly string[] SensitiveKeyMarkers =
    [
        "password",
        "secret",
        "connectionstring",
        "apikey",
        "token",
        "credential",
        "auth"
    ];

    private readonly IConfiguration configuration;
    private readonly IHostEnvironment environment;
    private readonly HealthCheckService healthCheckService;
    private readonly ILogger<ActuatorController> logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ActuatorController"/> class.
    /// </summary>
    public ActuatorController(
        IConfiguration configuration,
        IHostEnvironment environment,
        HealthCheckService healthCheckService,
        ILogger<ActuatorController> logger)
    {
        this.configuration = configuration;
        this.environment = environment;
        this.healthCheckService = healthCheckService;
        this.logger = logger;
    }

    /// <summary>
    /// Returns the list of available Actuator-style endpoints and their URLs.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GetEndpoints()
    {
        var baseUrl = $"{this.Request.Scheme}://{this.Request.Host.Value}";

        var response = new
        {
            _links = new
            {
                self = new { href = $"{baseUrl}/actuator" },
                health = new { href = $"{baseUrl}/actuator/health" },
                info = new { href = $"{baseUrl}/actuator/info" },
                env = new { href = $"{baseUrl}/actuator/env" },
            },
        };

        return this.Ok(response);
    }

    /// <summary>
    /// Provides high-level application information similar to Spring Boot Actuator's /actuator/info.
    /// </summary>
    [HttpGet("info")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GetInfo()
    {
        var assembly = System.Reflection.Assembly.GetEntryAssembly();
        var informationalVersion = assembly?
            .GetCustomAttributes(typeof(System.Reflection.AssemblyInformationalVersionAttribute), false)
            .OfType<System.Reflection.AssemblyInformationalVersionAttribute>()
            .FirstOrDefault()
            ?.InformationalVersion;

        var info = new
        {
            app = new
            {
                name = assembly?.GetName().Name ?? "SpringJavaEquivalent",
                version = informationalVersion ?? assembly?.GetName().Version?.ToString() ?? "unknown",
                environment = this.environment.EnvironmentName,
            },
            runtime = new
            {
                framework = RuntimeInformation.FrameworkDescription,
                os = RuntimeInformation.OSDescription,
                processArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
            },
            serverTimeUtc = DateTimeOffset.UtcNow,
        };

        return this.Ok(info);
    }

    /// <summary>
    /// Exposes a sanitized view of configuration/environment data similar to Actuator's /actuator/env.
    /// </summary>
    [HttpGet("env")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public IActionResult GetEnvironment()
    {
        var properties = this.configuration
            .AsEnumerable(makePathsRelative: true)
            .Where(kv => !string.IsNullOrWhiteSpace(kv.Value) && !IsSensitiveKey(kv.Key))
            .OrderBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(kv => kv.Key, kv => kv.Value, StringComparer.OrdinalIgnoreCase);

        var response = new
        {
            activeProfiles = new[] { this.environment.EnvironmentName },
            propertySources = new object[]
            {
                new
                {
                    name = "applicationConfiguration",
                    properties,
                },
            },
        };

        return this.Ok(response);
    }

    /// <summary>
    /// Runs registered health checks and returns a Spring-style health report.
    /// </summary>
    [HttpGet("health")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(object), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetHealthAsync(CancellationToken cancellationToken)
    {
        var report = await this.healthCheckService.CheckHealthAsync(null, cancellationToken);

        var components = report.Entries.ToDictionary(
            entry => entry.Key,
            entry => new
            {
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description,
                duration = entry.Value.Duration,
                tags = entry.Value.Tags,
                data = entry.Value.Data,
            },
            StringComparer.OrdinalIgnoreCase);

        var response = new
        {
            status = report.Status.ToString(),
            details = components,
        };

        var statusCode = report.Status switch
        {
            HealthStatus.Unhealthy => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status200OK,
        };

        this.logger.LogInformation("Actuator health status: {Status}", report.Status);

        return this.StatusCode(statusCode, response);
    }

    private static bool IsSensitiveKey(string key)
        => SensitiveKeyMarkers.Any(marker => key.Contains(marker, StringComparison.OrdinalIgnoreCase));
}
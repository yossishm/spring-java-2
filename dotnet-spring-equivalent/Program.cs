// <copyright file="Program.cs" company="SpringJavaEquivalent">
// Copyright (c) 2024. All rights reserved.
// </copyright>

using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Prometheus;
using SpringJavaEquivalent.Authorization;
using SpringJavaEquivalent.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Enhanced .NET JWT Authorization API",
        Version = "v1",
        Description = """
            ## Enhanced JWT Authorization API Documentation

            This API demonstrates comprehensive JWT-based authorization with **8 security levels** that now match Spring capabilities:

            ### Authorization Levels (.NET vs Spring):
            0. **Public Access** - No authentication required (.NET ✅, Spring ✅)
            1. **Basic Authentication** - Valid JWT token needed (.NET ✅, Spring ✅)
            2. **Role-Based Access** - Specific roles required (.NET ✅, Spring ✅)
            3. **Admin Role Access** - ADMIN role required (.NET ✅, Spring ✅)
            4. **Permission-Based Access** - Specific permissions required (.NET ✅, Spring ✅)
            5. **Authentication Level Access** - AAL2+ required (.NET ✅, Spring ✅)
            6. **Identity Provider Access** - Enterprise IDP required (.NET ✅, Spring ✅)
            7. **Multi-Factor Authorization** - Complex combinations (.NET ✅, Spring ✅)

            ### Available Permissions:
            - `CACHE_READ` - Read cache operations
            - `CACHE_WRITE` - Write cache operations
            - `CACHE_DELETE` - Delete cache operations
            - `ADMIN_ACCESS` - Administrative operations

            ### Available Roles:
            - `USER` - Basic user access
            - `ADMIN` - Administrative access
            - `MANAGER` - Management access
            - `READONLY` - Read-only access
            - `WRITER` - Write access

            ### Authentication Levels:
            - `AAL1` - Basic authentication
            - `AAL2` - Multi-factor authentication
            - `AAL3` - Hardware-based authentication

            ### Identity Providers:
            - `local` - Local authentication
            - `enterprise` - Enterprise SSO
            - `external` - External provider
            """,
    });
});

// Add JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT secret must be configured"))),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero,
        };
    });

// Add Authorization
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CacheReadPolicy", policy =>
        policy.Requirements.Add(new PermissionRequirement("CACHE_READ")))
    .AddPolicy("CacheWritePolicy", policy =>
        policy.Requirements.Add(new PermissionRequirement("CACHE_WRITE")))
    .AddPolicy("CacheDeletePolicy", policy =>
        policy.Requirements.Add(new PermissionRequirement("CACHE_DELETE")))
    .AddPolicy("IdentityProviderPolicy", policy =>
        policy.Requirements.Add(new IdentityProviderRequirement("enterprise")))
    .AddPolicy("AuthLevelPolicy", policy =>
        policy.Requirements.Add(new AuthLevelRequirement("AAL2")))
    .AddPolicy("RequireCacheReadOrAdminPermission", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("permission", "CACHE_READ") ||
            context.User.IsInRole("ADMIN")))
    .AddPolicy("RequireCacheWriteOrAdminPermission", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("permission", "CACHE_WRITE") ||
            context.User.IsInRole("ADMIN")))
    .AddPolicy("RequireCacheDeleteOrAdminPermission", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("permission", "CACHE_DELETE") ||
            context.User.IsInRole("ADMIN")));

// Add Authorization Handlers
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddScoped<IAuthorizationHandler, AnyPermissionHandler>();
builder.Services.AddScoped<IAuthorizationHandler, AllPermissionsHandler>();
builder.Services.AddScoped<IAuthorizationHandler, RoleHandler>();
builder.Services.AddScoped<IAuthorizationHandler, IdentityProviderHandler>();
builder.Services.AddScoped<IAuthorizationHandler, AuthLevelHandler>();

// Add custom services
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<LocalRestClient>();

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), new[] { "ready" });

// Add OpenTelemetry
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService("SpringJavaEquivalent", serviceVersion: "1.0.0")
        .AddAttributes(new Dictionary<string, object>
        {
            ["deployment.environment"] = builder.Environment.EnvironmentName,
        }))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddOtlpExporter());

// Add Prometheus metrics (skip in test environment)
if (!builder.Environment.EnvironmentName.Equals("Testing", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton<MetricServer>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add Prometheus metrics endpoint
app.UseHttpMetrics();

// Add authentication and authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Map health check endpoints
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false,
});

// Map Prometheus metrics endpoint
app.MapMetrics();

// Start the Prometheus metrics server (skip in test environment)
if (!app.Environment.EnvironmentName.Equals("Testing", StringComparison.OrdinalIgnoreCase))
{
    var metricServer = app.Services.GetRequiredService<MetricServer>();
    metricServer.Start();
}

app.Run();

// Make Program class accessible for testing
public partial class Program
{
    private const string ConstructorMessage = "Program constructor called";

    protected Program()
    {
        #pragma warning disable CA1303 // Do not pass literals as localized parameters
        Console.WriteLine(ConstructorMessage);
        #pragma warning restore CA1303 // Do not pass literals as localized parameters
    }
}

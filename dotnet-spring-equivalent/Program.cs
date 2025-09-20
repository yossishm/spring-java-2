using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using System.Text;
using System.Text.Json;
using Prometheus;
using SpringJavaEquivalent.Services;
using SpringJavaEquivalent.Authorization;

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
            - `CACHE_ADMIN` - Full cache administration
            
            ### Available Roles:
            - `ADMIN` - Full system access
            - `USER` - Basic user access
            
            ### Authentication Assurance Levels (AAL):
            - `AAL1` - Single-factor authentication (password only)
            - `AAL2` - Multi-factor authentication (password + MFA)
            - `AAL3` - Hardware-based authentication (FIDO2, smart cards)
            
            ### Identity Providers (IDP):
            - `local` - Local authentication
            - `azure-ad` - Microsoft Azure Active Directory
            - `okta` - Okta Identity Provider
            - `enterprise-ldap` - Enterprise LDAP
            
            ### Getting Started:
            1. Generate a JWT token using `/api/v1/auth/token/{type}`
            2. Use the token in the Authorization header: `Bearer <token>`
            3. Test different authorization levels using `/api/v1/enhanced-test/*`
            
            ### .NET Now Matches Spring:
            - ✅ Real JWT implementation with proper validation
            - ✅ 8 authorization levels with comprehensive policies
            - ✅ Role-based and permission-based access control
            - ✅ Authentication Assurance Levels (AAL)
            - ✅ Identity Provider tracking
            - ✅ Multi-factor authorization
            - ✅ Comprehensive security documentation
            """
    });

    // Add JWT Bearer authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = """
            JWT Authorization header using the Bearer scheme.
            
            **Example:** `Authorization: Bearer <token>`
            
            **Token Generation:**
            - Use `/api/v1/auth/token/admin` for admin access
            - Use `/api/v1/auth/token/user` for basic user access
            - Use `/api/v1/auth/token/cache-reader` for read-only access
            - Use `/api/v1/auth/token/cache-writer` for read/write access
            - Use `/api/v1/auth/token/cache-admin` for full cache access
            """,
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add JWT service
builder.Services.AddScoped<JwtService>();

// Configure JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "mySecretKeyForJWTTokenGenerationAndValidation123456789";
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // For development
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Configure Authorization with custom policies
builder.Services.AddAuthorization(options =>
{
    // Permission-based policies
    options.AddPolicy("RequireCacheReadPermission", policy =>
        policy.Requirements.Add(new PermissionRequirement("CACHE_READ")));
    
    options.AddPolicy("RequireCacheWritePermission", policy =>
        policy.Requirements.Add(new PermissionRequirement("CACHE_WRITE")));
    
    options.AddPolicy("RequireCacheDeletePermission", policy =>
        policy.Requirements.Add(new PermissionRequirement("CACHE_DELETE")));
    
    options.AddPolicy("RequireCacheAdminPermission", policy =>
        policy.Requirements.Add(new PermissionRequirement("CACHE_ADMIN")));

    // Combined permission policies
    options.AddPolicy("RequireCacheReadOrAdminPermission", policy =>
        policy.Requirements.Add(new AnyPermissionRequirement("CACHE_READ", "CACHE_ADMIN")));
    
    options.AddPolicy("RequireCacheWriteOrAdminPermission", policy =>
        policy.Requirements.Add(new AnyPermissionRequirement("CACHE_WRITE", "CACHE_ADMIN")));
    
    options.AddPolicy("RequireCacheDeleteOrAdminPermission", policy =>
        policy.Requirements.Add(new AnyPermissionRequirement("CACHE_DELETE", "CACHE_ADMIN")));

    // Authentication level policies
    options.AddPolicy("RequireAAL2OrHigher", policy =>
        policy.Requirements.Add(new AuthLevelRequirement("AAL2")));
    
    options.AddPolicy("RequireAAL3OrHigher", policy =>
        policy.Requirements.Add(new AuthLevelRequirement("AAL3")));

    // Identity provider policies
    options.AddPolicy("RequireEnterpriseIdp", policy =>
        policy.Requirements.Add(new IdentityProviderRequirement("enterprise-ldap")));

    // Multi-factor authorization policy
    options.AddPolicy("RequireMultiFactorAuth", policy =>
    {
        policy.RequireRole("ADMIN");
        policy.Requirements.Add(new PermissionRequirement("CACHE_ADMIN"));
        policy.Requirements.Add(new AnyPermissionRequirement("AUTH_LEVEL_AAL3", "IDP_enterprise-ldap"));
    });
});

// Register authorization handlers
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddScoped<IAuthorizationHandler, AnyPermissionHandler>();
builder.Services.AddScoped<IAuthorizationHandler, AllPermissionsHandler>();
builder.Services.AddScoped<IAuthorizationHandler, AuthLevelHandler>();
builder.Services.AddScoped<IAuthorizationHandler, IdentityProviderHandler>();

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), new[] { "ready" })
    .AddCheck("liveness", () => HealthCheckResult.Healthy(), new[] { "live" });

// Add OpenTelemetry with simple configuration
var otelEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT") ?? "http://otel-collector:4317";
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService(serviceName: "dotnet-app", serviceVersion: "1.0.0"))
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri(otelEndpoint);
                options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
            });
    });

var app = builder.Build();

// Log OTEL configuration status
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("🔧 OpenTelemetry configuration completed successfully");
logger.LogInformation("📊 Metrics configured with OTLP exporter to {Endpoint}", otelEndpoint);
logger.LogInformation("🚀 DOCKER BUILD TEST - This message should appear in container logs!");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Disable HTTPS redirection for K8s

// Add Prometheus metrics middleware
app.UseHttpMetrics();
app.UseRouting();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoints
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});

app.MapHealthChecks("/actuator/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            status = report.Status.ToString(),
            components = report.Entries.ToDictionary(
                entry => entry.Key,
                entry => new
                {
                    status = entry.Value.Status.ToString(),
                    details = entry.Value.Description
                }
            )
        };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
});

// Add Prometheus metrics endpoint
app.MapMetrics();

app.Run();

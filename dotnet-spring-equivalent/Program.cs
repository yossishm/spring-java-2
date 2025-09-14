using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), new[] { "ready" })
    .AddCheck("liveness", () => HealthCheckResult.Healthy(), new[] { "live" });

// Add OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource("SpringJavaEquivalent")
            .SetResourceBuilder(ResourceBuilder.CreateDefault()
                .AddService("SpringJavaEquivalent", "1.0.0"));
    })
    .WithMetrics(metrics =>
    {
        metrics
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Disable HTTPS redirection for K8s
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

// Add metrics endpoint for Prometheus
app.MapGet("/metrics", async (HttpContext context) =>
{
    context.Response.ContentType = "text/plain; version=0.0.4; charset=utf-8";
    
    // Simple metrics in Prometheus format
    var metrics = $@"# HELP dotnet_http_requests_total Total HTTP requests
# TYPE dotnet_http_requests_total counter
dotnet_http_requests_total{{method=""GET"",endpoint=""/""}} 100
dotnet_http_requests_total{{method=""GET"",endpoint=""/health""}} 50

# HELP dotnet_http_request_duration_seconds HTTP request duration
# TYPE dotnet_http_request_duration_seconds histogram
dotnet_http_request_duration_seconds_bucket{{le=""0.1""}} 120
dotnet_http_request_duration_seconds_bucket{{le=""0.5""}} 140
dotnet_http_request_duration_seconds_bucket{{le=""1.0""}} 145
dotnet_http_request_duration_seconds_bucket{{le=""+Inf""}} 150
dotnet_http_request_duration_seconds_sum 45.2
dotnet_http_request_duration_seconds_count 150

# HELP dotnet_memory_usage_bytes Memory usage in bytes
# TYPE dotnet_memory_usage_bytes gauge
dotnet_memory_usage_bytes 52428800

# HELP dotnet_cpu_usage_percent CPU usage percentage
# TYPE dotnet_cpu_usage_percent gauge
dotnet_cpu_usage_percent 15.5
";
    
    await context.Response.WriteAsync(metrics);
});

app.Run();

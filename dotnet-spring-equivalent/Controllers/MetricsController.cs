// <copyright file="MetricsController.cs" company="SpringJavaEquivalent">
// Copyright (c) 2024. All rights reserved.
// </copyright>

namespace SpringJavaEquivalent.Controllers;

using System.Diagnostics;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class MetricsController : ControllerBase
{
    private static readonly Meter Meter = new("SpringJavaEquivalent.Metrics", "1.0.0");
    private static readonly Counter<long> RequestCounter = Meter.CreateCounter<long>("custom_requests_total", "Total number of custom requests");
    private static readonly Histogram<double> ResponseTime = Meter.CreateHistogram<double>("custom_response_time", "Custom response time");
    private static readonly Random Random = new();

    [HttpGet("test")]
    public async Task<string> TestMetrics()
    {
        RequestCounter.Add(1);

        using var activity = new Activity("custom_operation");
        activity.Start();

        try
        {
            // Simulate some work
            await Task.Delay(Random.Next(50, 150));
            return "Metrics test completed";
        }
        finally
        {
            activity.Stop();
            ResponseTime.Record(activity.Duration.TotalMilliseconds);
        }
    }

    [HttpGet("counter")]
    public string IncrementCounter([FromQuery] int value = 1)
    {
        RequestCounter.Add(value);
        return $"Counter incremented by {value}";
    }

    [HttpGet("slow")]
    public async Task<string> SlowEndpoint()
    {
        RequestCounter.Add(1);

        using var activity = new Activity("slow_operation");
        activity.Start();

        try
        {
            // Simulate slow operation
            await Task.Delay(Random.Next(500, 1500));
            return "Slow operation completed";
        }
        finally
        {
            activity.Stop();
            ResponseTime.Record(activity.Duration.TotalMilliseconds);
        }
    }
}
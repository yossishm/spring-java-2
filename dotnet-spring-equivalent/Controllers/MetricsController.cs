using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Metrics;
using System.Diagnostics;

namespace SpringJavaEquivalent.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetricsController : ControllerBase
    {
        private static readonly Meter _meter = new("SpringJavaEquivalent.Metrics", "1.0.0");
        private static readonly Counter<long> _requestCounter = _meter.CreateCounter<long>("custom_requests_total", "Total number of custom requests");
        private static readonly Histogram<double> _responseTime = _meter.CreateHistogram<double>("custom_response_time", "Custom response time");
        private static readonly Random _random = new();

        [HttpGet("test")]
        public async Task<string> TestMetrics()
        {
            _requestCounter.Add(1);
            
            using var activity = new Activity("custom_operation");
            activity.Start();
            
            try
            {
                // Simulate some work
                await Task.Delay(_random.Next(50, 150));
                return "Metrics test completed";
            }
            finally
            {
                activity.Stop();
                _responseTime.Record(activity.Duration.TotalMilliseconds);
            }
        }

        [HttpGet("counter")]
        public string IncrementCounter([FromQuery] int value = 1)
        {
            _requestCounter.Add(value);
            return $"Counter incremented by {value}";
        }

        [HttpGet("slow")]
        public async Task<string> SlowEndpoint()
        {
            _requestCounter.Add(1);
            
            using var activity = new Activity("slow_operation");
            activity.Start();
            
            try
            {
                // Simulate slow operation
                await Task.Delay(_random.Next(500, 1500));
                return "Slow operation completed";
            }
            finally
            {
                activity.Stop();
                _responseTime.Record(activity.Duration.TotalMilliseconds);
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SpringJavaEquivalent.Controllers;
using Xunit;

namespace SpringJavaEquivalent.Tests;

public class MetricsControllerTests
{
    private readonly MetricsController _controller;

    public MetricsControllerTests()
    {
        this._controller = new MetricsController();
    }

    [Fact]
    public async Task TestMetrics_ShouldReturnString()
    {
        // Act
        var result = await this._controller.TestMetrics();

        // Assert
        Assert.IsType<string>(result);
        Assert.Equal("Metrics test completed", result);
    }

    [Fact]
    public void IncrementCounter_ShouldReturnString()
    {
        // Act
        var result = this._controller.IncrementCounter(5);

        // Assert
        Assert.IsType<string>(result);
        Assert.Equal("Counter incremented by 5", result);
    }

    [Fact]
    public async Task SlowEndpoint_ShouldReturnString()
    {
        // Act
        var result = await this._controller.SlowEndpoint();

        // Assert
        Assert.IsType<string>(result);
        Assert.Equal("Slow operation completed", result);
    }
}
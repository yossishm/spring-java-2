// <copyright file="SimpleWorkingTests.cs" company="SpringJavaEquivalent">
// Copyright (c) SpringJavaEquivalent. All rights reserved.
// </copyright>

namespace SpringJavaEquivalent.Tests;

using Microsoft.AspNetCore.Mvc;
using SpringJavaEquivalent.Controllers;
using Xunit;

public class SimpleWorkingTests
{
    [Fact]
    public void ApplicationController_Home_ShouldReturnOkResult()
    {
        // Arrange
        var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ApplicationController>();
        var controller = new ApplicationController(logger);

        // Act
        var result = controller.Home();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void ApplicationController_GetObject_ShouldReturnOkResult()
    {
        // Arrange
        var logger = new Microsoft.Extensions.Logging.Abstractions.NullLogger<ApplicationController>();
        var controller = new ApplicationController(logger);

        // Act
        var result = controller.GetObject("test-id");

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task MetricsController_TestMetrics_ShouldReturnString()
    {
        // Arrange
        var controller = new MetricsController();

        // Act
        var result = await controller.TestMetrics();

        // Assert
        Assert.IsType<string>(result);
        Assert.Equal("Metrics test completed", result);
    }

    [Fact]
    public void MetricsController_IncrementCounter_ShouldReturnString()
    {
        // Arrange
        var controller = new MetricsController();

        // Act
        var result = controller.IncrementCounter(5);

        // Assert
        Assert.IsType<string>(result);
        Assert.Equal("Counter incremented by 5", result);
    }

    [Fact]
    public void VulnerableJwtController_CreateToken_ShouldReturnOkResult()
    {
        // Arrange
        var controller = new VulnerableJwtController();
        var payload = new Dictionary<string, object>
        {
            { "username", "testuser" },
            { "role", "admin" },
        };

        // Act
        var result = controller.CreateToken(payload);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public void EnhancedAuthTestController_PublicEndpoint_ShouldReturnOkResult()
    {
        // Arrange
        var controller = new EnhancedAuthTestController();

        // Act
        var result = controller.PublicEndpoint();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
}

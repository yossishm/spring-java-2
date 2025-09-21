using Microsoft.AspNetCore.Mvc;
using SpringJavaEquivalent.Controllers;
using Xunit;

namespace SpringJavaEquivalent.Tests;

public class VulnerableJwtControllerTests
{
    private readonly VulnerableJwtController _controller;

    public VulnerableJwtControllerTests()
    {
        _controller = new VulnerableJwtController();
    }

    [Fact]
    public void CreateToken_WithValidPayload_ShouldReturnOkResult()
    {
        // Arrange
        var payload = new Dictionary<string, object>
        {
            { "username", "testuser" },
            { "role", "admin" }
        };

        // Act
        var result = _controller.CreateToken(payload);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult!.Value);
        Assert.IsType<string>(okResult.Value);
    }

    [Fact]
    public void VerifyToken_WithValidToken_ShouldReturnOkResult()
    {
        // Arrange
        var request = new Dictionary<string, string>
        {
            { "token", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c" }
        };

        // Act
        var result = _controller.VerifyToken(request);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult!.Value);
    }

    [Fact]
    public void DecodeToken_WithValidToken_ShouldReturnOkResult()
    {
        // Arrange
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

        // Act
        var result = _controller.DecodeToken(token);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult!.Value);
    }

    [Fact]
    public void VerifyAnyAlgorithm_WithValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var request = new Dictionary<string, string>
        {
            { "token", "test-token" },
            { "algorithm", "HS256" }
        };

        // Act
        var result = _controller.VerifyAnyAlgorithm(request);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult!.Value);
    }
}
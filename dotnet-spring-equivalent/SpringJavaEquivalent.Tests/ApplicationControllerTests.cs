namespace SpringJavaEquivalent.Tests;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using SpringJavaEquivalent.Controllers;
using System.Security.Claims;
using Xunit;

public class ApplicationControllerTests
{
    private readonly Mock<ILogger<ApplicationController>> _mockLogger;
    private readonly ApplicationController _controller;

    public ApplicationControllerTests()
    {
        _mockLogger = new Mock<ILogger<ApplicationController>>();
        _controller = new ApplicationController(_mockLogger.Object);
        
        // Setup controller context
        var context = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.ControllerContext = context;
    }

    [Fact]
    public void Home_ShouldReturnOkResult()
    {
        // Act
        var result = _controller.Home();

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.Equal("Hello Docker Yossi World", okResult!.Value);
    }

    [Fact]
    public void GetObject_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var id = "test-id";
        SetupAuthenticatedUser();

        // Act
        var result = _controller.GetObject(id);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.Equal("get", okResult!.Value);
    }

    [Fact]
    public void PutObject_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var id = "test-id";
        SetupAuthenticatedUser();

        // Act
        var result = _controller.PutObject(id);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.Equal("put", okResult!.Value);
    }

    [Fact]
    public void DeleteObject_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var id = "test-id";
        SetupAuthenticatedUser();

        // Act
        var result = _controller.DeleteObject(id);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.Equal("delete", okResult!.Value);
    }

    private void SetupAuthenticatedUser()
    {
        var claims = new List<Claim>
        {
            new Claim("permission", "CACHE_READ"),
            new Claim("permission", "CACHE_WRITE"),
        };
        
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
        
        _controller.ControllerContext.HttpContext.User = principal;
    }
}
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SpringJavaEquivalent.Controllers;
using System.Security.Claims;
using Xunit;

namespace SpringJavaEquivalent.Tests;

public class EnhancedAuthTestControllerTests
{
    private readonly EnhancedAuthTestController _controller;

    public EnhancedAuthTestControllerTests()
    {
        _controller = new EnhancedAuthTestController();
        
        // Setup controller context
        var context = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        _controller.ControllerContext = context;
    }

    [Fact]
    public void PublicEndpoint_ShouldReturnOkResult()
    {
        // Act
        var result = _controller.PublicEndpoint();

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult!.Value);
    }

    [Fact]
    public void AuthenticatedEndpoint_WithAuthenticatedUser_ShouldReturnOkResult()
    {
        // Arrange
        SetupAuthenticatedUser();

        // Act
        var result = _controller.AuthenticatedEndpoint();

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult!.Value);
    }

    [Fact]
    public void UserRoleEndpoint_WithUserRole_ShouldReturnOkResult()
    {
        // Arrange
        SetupUserRole();

        // Act
        var result = _controller.UserRoleEndpoint();

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult!.Value);
    }

    [Fact]
    public void AdminRoleEndpoint_WithAdminRole_ShouldReturnOkResult()
    {
        // Arrange
        SetupAdminRole();

        // Act
        var result = _controller.AdminRoleEndpoint();

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult!.Value);
    }

    [Fact]
    public void CacheReadEndpoint_WithReadPermission_ShouldReturnOkResult()
    {
        // Arrange
        SetupReadPermission();

        // Act
        var result = _controller.CacheReadEndpoint();

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult!.Value);
    }

    [Fact]
    public void CacheWriteEndpoint_WithWritePermission_ShouldReturnOkResult()
    {
        // Arrange
        SetupWritePermission();

        // Act
        var result = _controller.CacheWriteEndpoint();

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult!.Value);
    }

    private void SetupAuthenticatedUser()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "testuser")
        };
        
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
        
        _controller.ControllerContext.HttpContext.User = principal;
    }

    private void SetupUserRole()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim(ClaimTypes.Role, "USER")
        };
        
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
        
        _controller.ControllerContext.HttpContext.User = principal;
    }

    private void SetupAdminRole()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.Role, "ADMIN")
        };
        
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
        
        _controller.ControllerContext.HttpContext.User = principal;
    }

    private void SetupReadPermission()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim("permission", "CACHE_READ")
        };
        
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
        
        _controller.ControllerContext.HttpContext.User = principal;
    }

    private void SetupWritePermission()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, "testuser"),
            new Claim("permission", "CACHE_WRITE")
        };
        
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        var principal = new ClaimsPrincipal(identity);
        
        _controller.ControllerContext.HttpContext.User = principal;
    }
}

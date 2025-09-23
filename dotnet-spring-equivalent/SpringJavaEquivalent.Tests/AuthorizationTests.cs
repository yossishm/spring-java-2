namespace SpringJavaEquivalent.Tests;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using SpringJavaEquivalent.Authorization;
using System.Security.Claims;
using Xunit;

public class PermissionRequirementTests
{
    [Fact]
    public void Constructor_WithValidPermission_ShouldCreateInstance()
    {
        // Arrange
        var permission = "READ";

        // Act
        var requirement = new PermissionRequirement(permission);

        // Assert
        Assert.Equal(permission, requirement.Permission);
    }

    [Fact]
    public void Constructor_WithNullPermission_ShouldThrowArgumentNullException()
    {
        // Arrange
        string permission = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PermissionRequirement(permission));
    }

    [Fact]
    public void Constructor_WithEmptyPermission_ShouldThrowArgumentNullException()
    {
        // Arrange
        var permission = string.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PermissionRequirement(permission));
    }
}

public class PermissionHandlerTests
{
    private readonly PermissionHandler _handler;

    public PermissionHandlerTests()
    {
        this._handler = new PermissionHandler();
    }

    [Fact]
    public async Task HandleAsync_WithMatchingPermission_ShouldSucceed()
    {
        // Arrange
        var requirement = new PermissionRequirement("READ");
        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            CreateUserWithPermission("READ"),
            null!);

        // Act
        await this._handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WithNonMatchingPermission_ShouldFail()
    {
        // Arrange
        var requirement = new PermissionRequirement("WRITE");
        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            CreateUserWithPermission("READ"),
            null!);

        // Act
        await this._handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WithMultiplePermissions_ShouldSucceed()
    {
        // Arrange
        var requirement = new PermissionRequirement("READ");
        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            CreateUserWithPermissions(new[] { "READ", "WRITE" }),
            null!);

        // Act
        await this._handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WithNoPermissions_ShouldFail()
    {
        // Arrange
        var requirement = new PermissionRequirement("READ");
        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            CreateUserWithoutPermissions(),
            null!);

        // Act
        await this._handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WithNullUser_ShouldFail()
    {
        // Arrange
        var requirement = new PermissionRequirement("READ");
        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            null!,
            null!);

        // Act
        await this._handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WithEmptyPermissionClaim_ShouldFail()
    {
        // Arrange
        var requirement = new PermissionRequirement("READ");
        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            CreateUserWithPermission(string.Empty),
            null!);

        // Act
        await this._handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WithNullPermissionClaim_ShouldFail()
    {
        // Arrange
        var requirement = new PermissionRequirement("READ");
        var context = new AuthorizationHandlerContext(
            new[] { requirement },
            CreateUserWithPermission(null!),
            null!);

        // Act
        await this._handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    private static ClaimsPrincipal CreateUserWithPermission(string permission)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "testuser"),
        };
        
        if (permission != null)
        {
            claims.Add(new("permission", permission));
        }
        
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        return new ClaimsPrincipal(identity);
    }

    private static ClaimsPrincipal CreateUserWithPermissions(string[] permissions)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "testuser"),
        };
        
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }
        
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        return new ClaimsPrincipal(identity);
    }

    private static ClaimsPrincipal CreateUserWithoutPermissions()
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, "testuser"),
        };
        var identity = new ClaimsIdentity(claims, "TestAuthType");
        return new ClaimsPrincipal(identity);
    }
}

// <copyright file="AdditionalAuthorizationTests.cs" company="SpringJavaEquivalent">
// Copyright (c) 2024. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Authorization;
using SpringJavaEquivalent.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace SpringJavaEquivalent.Tests;

public class AnyPermissionHandlerTests
{
    private readonly AnyPermissionHandler _handler;

    public AnyPermissionHandlerTests()
    {
        _handler = new AnyPermissionHandler();
    }

    [Fact]
    public async Task HandleAsync_WithAnyMatchingPermission_ShouldSucceed()
    {
        // Arrange
        var requirement = new AnyPermissionRequirement("read", "write");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("permission", "read") }, "TestAuth"));
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WithNoMatchingPermission_ShouldNotSucceed()
    {
        // Arrange
        var requirement = new AnyPermissionRequirement("read", "write");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("permission", "delete") }, "TestAuth"));
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WithNullUser_ShouldNotSucceed()
    {
        // Arrange
        var requirement = new AnyPermissionRequirement("read", "write");
        var context = new AuthorizationHandlerContext(new[] { requirement }, null!, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }
}

public class AllPermissionsHandlerTests
{
    private readonly AllPermissionsHandler _handler;

    public AllPermissionsHandlerTests()
    {
        _handler = new AllPermissionsHandler();
    }

    [Fact]
    public async Task HandleAsync_WithAllMatchingPermissions_ShouldSucceed()
    {
        // Arrange
        var requirement = new AllPermissionsRequirement("read", "write");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] 
        { 
            new Claim("permission", "read"), 
            new Claim("permission", "write") 
        }, "TestAuth"));
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WithMissingPermission_ShouldNotSucceed()
    {
        // Arrange
        var requirement = new AllPermissionsRequirement("read", "write");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("permission", "read") }, "TestAuth"));
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WithNullUser_ShouldNotSucceed()
    {
        // Arrange
        var requirement = new AllPermissionsRequirement("read", "write");
        var context = new AuthorizationHandlerContext(new[] { requirement }, null!, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }
}

public class RoleHandlerTests
{
    private readonly RoleHandler _handler;

    public RoleHandlerTests()
    {
        _handler = new RoleHandler();
    }

    [Fact]
    public async Task HandleAsync_WithMatchingRole_ShouldSucceed()
    {
        // Arrange
        var requirement = new RoleRequirement("admin");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "admin") }, "TestAuth"));
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WithNonMatchingRole_ShouldNotSucceed()
    {
        // Arrange
        var requirement = new RoleRequirement("admin");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Role, "user") }, "TestAuth"));
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WithNullUser_ShouldNotSucceed()
    {
        // Arrange
        var requirement = new RoleRequirement("admin");
        var context = new AuthorizationHandlerContext(new[] { requirement }, null!, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }
}

public class IdentityProviderHandlerTests
{
    private readonly IdentityProviderHandler _handler;

    public IdentityProviderHandlerTests()
    {
        _handler = new IdentityProviderHandler();
    }

    [Fact]
    public async Task HandleAsync_WithMatchingIdentityProvider_ShouldSucceed()
    {
        // Arrange
        var requirement = new IdentityProviderRequirement("google");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("identity_provider", "google") }, "TestAuth"));
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WithNonMatchingIdentityProvider_ShouldNotSucceed()
    {
        // Arrange
        var requirement = new IdentityProviderRequirement("google");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("identity_provider", "azure") }, "TestAuth"));
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WithNullUser_ShouldNotSucceed()
    {
        // Arrange
        var requirement = new IdentityProviderRequirement("google");
        var context = new AuthorizationHandlerContext(new[] { requirement }, null!, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WithMissingIdentityProviderClaim_ShouldNotSucceed()
    {
        // Arrange
        var requirement = new IdentityProviderRequirement("google");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("other_claim", "value") }, "TestAuth"));
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }
}

public class AuthLevelHandlerTests
{
    private readonly AuthLevelHandler _handler;

    public AuthLevelHandlerTests()
    {
        _handler = new AuthLevelHandler();
    }

    [Fact]
    public async Task HandleAsync_WithMatchingAuthLevel_ShouldSucceed()
    {
        // Arrange
        var requirement = new AuthLevelRequirement("level1");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("auth_level", "level1") }, "TestAuth"));
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.True(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WithNonMatchingAuthLevel_ShouldNotSucceed()
    {
        // Arrange
        var requirement = new AuthLevelRequirement("level1");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("auth_level", "level2") }, "TestAuth"));
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WithNullUser_ShouldNotSucceed()
    {
        // Arrange
        var requirement = new AuthLevelRequirement("level1");
        var context = new AuthorizationHandlerContext(new[] { requirement }, null!, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }

    [Fact]
    public async Task HandleAsync_WithMissingAuthLevelClaim_ShouldNotSucceed()
    {
        // Arrange
        var requirement = new AuthLevelRequirement("level1");
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim("other_claim", "value") }, "TestAuth"));
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        Assert.False(context.HasSucceeded);
    }
}

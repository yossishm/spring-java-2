// <copyright file="AuthorizationRequirementTests.cs" company="SpringJavaEquivalent">
// Copyright (c) 2024. All rights reserved.
// </copyright>

namespace SpringJavaEquivalent.Tests;

using SpringJavaEquivalent.Authorization;
using Xunit;

/// <summary>
/// Tests for Authorization Requirement classes to improve coverage
/// </summary>
public class AuthorizationRequirementTests
{
    [Fact]
    public void PermissionRequirement_Constructor_WithValidPermission_ShouldSetProperty()
    {
        // Arrange
        var permission = "read";

        // Act
        var requirement = new PermissionRequirement(permission);

        // Assert
        Assert.Equal(permission, requirement.Permission);
    }

    [Fact]
    public void PermissionRequirement_Constructor_WithNullPermission_ShouldThrowArgumentNullException()
    {
        // Arrange
        string permission = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PermissionRequirement(permission));
    }

    [Fact]
    public void PermissionRequirement_Constructor_WithEmptyPermission_ShouldThrowArgumentException()
    {
        // Arrange
        var permission = string.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PermissionRequirement(permission));
    }

    [Fact]
    public void PermissionRequirement_Constructor_WithWhitespacePermission_ShouldThrowArgumentException()
    {
        // Arrange
        var permission = "   ";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new PermissionRequirement(permission));
    }

    [Fact]
    public void AnyPermissionRequirement_Constructor_WithValidPermissions_ShouldSetProperty()
    {
        // Arrange
        var permissions = new[] { "read", "write" };

        // Act
        var requirement = new AnyPermissionRequirement(permissions);

        // Assert
        Assert.Equal(permissions, requirement.Permissions);
    }

    [Fact]
    public void AnyPermissionRequirement_Constructor_WithNullPermissions_ShouldThrowArgumentNullException()
    {
        // Arrange
        string[] permissions = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AnyPermissionRequirement(permissions));
    }

    [Fact]
    public void AnyPermissionRequirement_Constructor_WithEmptyPermissions_ShouldSetProperty()
    {
        // Arrange
        var permissions = Array.Empty<string>();

        // Act
        var requirement = new AnyPermissionRequirement(permissions);

        // Assert
        Assert.Equal(permissions, requirement.Permissions);
    }

    [Fact]
    public void AllPermissionsRequirement_Constructor_WithValidPermissions_ShouldSetProperty()
    {
        // Arrange
        var permissions = new[] { "read", "write" };

        // Act
        var requirement = new AllPermissionsRequirement(permissions);

        // Assert
        Assert.Equal(permissions, requirement.Permissions);
    }

    [Fact]
    public void AllPermissionsRequirement_Constructor_WithNullPermissions_ShouldThrowArgumentNullException()
    {
        // Arrange
        string[] permissions = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AllPermissionsRequirement(permissions));
    }

    [Fact]
    public void AllPermissionsRequirement_Constructor_WithEmptyPermissions_ShouldSetProperty()
    {
        // Arrange
        var permissions = Array.Empty<string>();

        // Act
        var requirement = new AllPermissionsRequirement(permissions);

        // Assert
        Assert.Equal(permissions, requirement.Permissions);
    }

    [Fact]
    public void AuthLevelRequirement_Constructor_WithValidAuthLevel_ShouldSetProperty()
    {
        // Arrange
        var authLevel = "level1";

        // Act
        var requirement = new AuthLevelRequirement(authLevel);

        // Assert
        Assert.Equal(authLevel, requirement.RequiredAuthLevel);
    }

    [Fact]
    public void AuthLevelRequirement_Constructor_WithNullAuthLevel_ShouldThrowArgumentNullException()
    {
        // Arrange
        string authLevel = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new AuthLevelRequirement(authLevel));
    }

    [Fact]
    public void AuthLevelRequirement_Constructor_WithEmptyAuthLevel_ShouldSetProperty()
    {
        // Arrange
        var authLevel = string.Empty;

        // Act
        var requirement = new AuthLevelRequirement(authLevel);

        // Assert
        Assert.Equal(authLevel, requirement.RequiredAuthLevel);
    }

    [Fact]
    public void IdentityProviderRequirement_Constructor_WithValidIdentityProvider_ShouldSetProperty()
    {
        // Arrange
        var identityProvider = "google";

        // Act
        var requirement = new IdentityProviderRequirement(identityProvider);

        // Assert
        Assert.Equal(identityProvider, requirement.RequiredIdentityProvider);
    }

    [Fact]
    public void IdentityProviderRequirement_Constructor_WithNullIdentityProvider_ShouldThrowArgumentNullException()
    {
        // Arrange
        string identityProvider = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new IdentityProviderRequirement(identityProvider));
    }

    [Fact]
    public void IdentityProviderRequirement_Constructor_WithEmptyIdentityProvider_ShouldSetProperty()
    {
        // Arrange
        var identityProvider = string.Empty;

        // Act
        var requirement = new IdentityProviderRequirement(identityProvider);

        // Assert
        Assert.Equal(identityProvider, requirement.RequiredIdentityProvider);
    }

    [Fact]
    public void RoleRequirement_Constructor_WithValidRole_ShouldSetProperty()
    {
        // Arrange
        var role = "admin";

        // Act
        var requirement = new RoleRequirement(role);

        // Assert
        Assert.Equal(role, requirement.Role);
    }

    [Fact]
    public void RoleRequirement_Constructor_WithNullRole_ShouldThrowArgumentNullException()
    {
        // Arrange
        string role = null!;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new RoleRequirement(role));
    }

    [Fact]
    public void RoleRequirement_Constructor_WithEmptyRole_ShouldSetProperty()
    {
        // Arrange
        var role = string.Empty;

        // Act
        var requirement = new RoleRequirement(role);

        // Assert
        Assert.Equal(role, requirement.Role);
    }
}

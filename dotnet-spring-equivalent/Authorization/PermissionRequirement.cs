// <copyright file="PermissionRequirement.cs" company="SpringJavaEquivalent">
// Copyright (c) 2024. All rights reserved.
// </copyright>

namespace SpringJavaEquivalent.Authorization;

using Microsoft.AspNetCore.Authorization;

/// <summary>
/// Authorization requirement for permission-based access control
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(permission);
        this.Permission = permission;
    }
}

/// <summary>
/// Authorization requirement for multiple permissions (ANY)
/// </summary>
public class AnyPermissionRequirement : IAuthorizationRequirement
{
    public string[] Permissions { get; }

    public AnyPermissionRequirement(params string[] permissions)
    {
        ArgumentNullException.ThrowIfNull(permissions);
        this.Permissions = permissions;
    }
}

/// <summary>
/// Authorization requirement for multiple permissions (ALL)
/// </summary>
public class AllPermissionsRequirement : IAuthorizationRequirement
{
    public string[] Permissions { get; }

    public AllPermissionsRequirement(params string[] permissions)
    {
        ArgumentNullException.ThrowIfNull(permissions);
        this.Permissions = permissions;
    }
}

/// <summary>
/// Authorization requirement for authentication level
/// </summary>
public class AuthLevelRequirement : IAuthorizationRequirement
{
    public string RequiredAuthLevel { get; }

    public AuthLevelRequirement(string requiredAuthLevel)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(requiredAuthLevel);
        this.RequiredAuthLevel = requiredAuthLevel;
    }
}

/// <summary>
/// Authorization requirement for identity provider
/// </summary>
public class IdentityProviderRequirement : IAuthorizationRequirement
{
    public string RequiredIdentityProvider { get; }

    public IdentityProviderRequirement(string requiredIdentityProvider)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(requiredIdentityProvider);
        this.RequiredIdentityProvider = requiredIdentityProvider;
    }
}

/// <summary>
/// Authorization requirement for role-based access control
/// </summary>
public class RoleRequirement : IAuthorizationRequirement
{
    public string Role { get; }

    public RoleRequirement(string role)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(role);
        this.Role = role;
    }
}
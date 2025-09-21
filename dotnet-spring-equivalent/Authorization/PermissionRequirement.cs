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
        this.Permission = permission ?? throw new ArgumentNullException(nameof(permission));
        if (string.IsNullOrWhiteSpace(permission))
        {
            throw new ArgumentException("Permission cannot be empty or whitespace", nameof(permission));
        }
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
        this.Permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
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
        this.Permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
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
        this.RequiredAuthLevel = requiredAuthLevel ?? throw new ArgumentNullException(nameof(requiredAuthLevel));
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
        this.RequiredIdentityProvider = requiredIdentityProvider ?? throw new ArgumentNullException(nameof(requiredIdentityProvider));
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
        this.Role = role ?? throw new ArgumentNullException(nameof(role));
    }
}
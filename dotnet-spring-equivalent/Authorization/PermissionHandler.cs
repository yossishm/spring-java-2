// <copyright file="PermissionHandler.cs" company="SpringJavaEquivalent">
// Copyright (c) 2024. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SpringJavaEquivalent.Authorization;

/// <summary>
/// Authorization handler for permission-based access control
/// </summary>
internal class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        var hasPermission = context.User.HasClaim("permission", requirement.Permission);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Authorization handler for multiple permissions (ANY)
/// </summary>
internal class AnyPermissionHandler : AuthorizationHandler<AnyPermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AnyPermissionRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        var hasAnyPermission = requirement.Permissions.Any(permission =>
            context.User.HasClaim("permission", permission));

        if (hasAnyPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Authorization handler for multiple permissions (ALL)
/// </summary>
internal class AllPermissionsHandler : AuthorizationHandler<AllPermissionsRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AllPermissionsRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        var hasAllPermissions = requirement.Permissions.All(permission =>
            context.User.HasClaim("permission", permission));

        if (hasAllPermissions)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Authorization handler for role-based access control
/// </summary>
internal class RoleHandler : AuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        var hasRole = context.User.IsInRole(requirement.Role);

        if (hasRole)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Authorization handler for identity provider requirements
/// </summary>
internal class IdentityProviderHandler : AuthorizationHandler<IdentityProviderRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IdentityProviderRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        var identityProvider = context.User.FindFirst("identity_provider")?.Value;

        if (identityProvider == requirement.RequiredIdentityProvider)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Authorization handler for authentication level requirements
/// </summary>
internal class AuthLevelHandler : AuthorizationHandler<AuthLevelRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthLevelRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        var authLevel = context.User.FindFirst("auth_level")?.Value;

        if (authLevel == requirement.RequiredAuthLevel)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
// <copyright file="BaseAuthorizationHandler.cs" company="SpringJavaEquivalent">
// Copyright (c) 2024. All rights reserved.
// </copyright>

namespace SpringJavaEquivalent.Authorization;

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

/// <summary>
/// Base authorization handler with common functionality
/// </summary>
public abstract class BaseAuthorizationHandler<T> : AuthorizationHandler<T>
    where T : IAuthorizationRequirement
{
    protected static bool IsUserAuthenticated(AuthorizationHandlerContext context)
    {
        return context.User?.Identity?.IsAuthenticated == true;
    }

    protected static void SucceedIf(AuthorizationHandlerContext context, T requirement, bool condition)
    {
        if (condition)
        {
            context.Succeed(requirement);
        }
    }
}

/// <summary>
/// Authorization handler for permission-based access control
/// </summary>
public class PermissionHandler : BaseAuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        if (!IsUserAuthenticated(context))
        {
            return Task.CompletedTask;
        }

        var hasPermission = context.User.HasClaim("permission", requirement.Permission);
        SucceedIf(context, requirement, hasPermission);

        return Task.CompletedTask;
    }
}

/// <summary>
/// Authorization handler for multiple permissions (ANY)
/// </summary>
public class AnyPermissionHandler : BaseAuthorizationHandler<AnyPermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AnyPermissionRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        if (!IsUserAuthenticated(context))
        {
            return Task.CompletedTask;
        }

        var hasAnyPermission = requirement.Permissions.Any(permission =>
            context.User.HasClaim("permission", permission));

        SucceedIf(context, requirement, hasAnyPermission);

        return Task.CompletedTask;
    }
}

/// <summary>
/// Authorization handler for multiple permissions (ALL)
/// </summary>
public class AllPermissionsHandler : BaseAuthorizationHandler<AllPermissionsRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AllPermissionsRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        if (!IsUserAuthenticated(context))
        {
            return Task.CompletedTask;
        }

        var hasAllPermissions = requirement.Permissions.All(permission =>
            context.User.HasClaim("permission", permission));

        SucceedIf(context, requirement, hasAllPermissions);

        return Task.CompletedTask;
    }
}

/// <summary>
/// Authorization handler for role-based access control
/// </summary>
public class RoleHandler : BaseAuthorizationHandler<RoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RoleRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        if (!IsUserAuthenticated(context))
        {
            return Task.CompletedTask;
        }

        var hasRole = context.User.IsInRole(requirement.Role);
        SucceedIf(context, requirement, hasRole);

        return Task.CompletedTask;
    }
}

/// <summary>
/// Authorization handler for identity provider requirements
/// </summary>
public class IdentityProviderHandler : BaseAuthorizationHandler<IdentityProviderRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IdentityProviderRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        if (!IsUserAuthenticated(context))
        {
            return Task.CompletedTask;
        }

        var identityProvider = context.User.FindFirst("identity_provider")?.Value;
        var hasRequiredIdp = identityProvider == requirement.RequiredIdentityProvider;
        SucceedIf(context, requirement, hasRequiredIdp);

        return Task.CompletedTask;
    }
}

/// <summary>
/// Authorization handler for authentication level requirements
/// </summary>
public class AuthLevelHandler : BaseAuthorizationHandler<AuthLevelRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthLevelRequirement requirement)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(requirement);

        if (!IsUserAuthenticated(context))
        {
            return Task.CompletedTask;
        }

        var authLevel = context.User.FindFirst("auth_level")?.Value;
        var hasRequiredAuthLevel = authLevel == requirement.RequiredAuthLevel;
        SucceedIf(context, requirement, hasRequiredAuthLevel);

        return Task.CompletedTask;
    }
}
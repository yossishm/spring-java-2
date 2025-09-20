using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SpringJavaEquivalent.Authorization;

/// <summary>
/// Authorization handler for permission-based access control
/// </summary>
public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
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
public class AnyPermissionHandler : AuthorizationHandler<AnyPermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AnyPermissionRequirement requirement)
    {
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
public class AllPermissionsHandler : AuthorizationHandler<AllPermissionsRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AllPermissionsRequirement requirement)
    {
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
/// Authorization handler for authentication level
/// </summary>
public class AuthLevelHandler : AuthorizationHandler<AuthLevelRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthLevelRequirement requirement)
    {
        var authLevelClaim = context.User.FindFirst("auth_level")?.Value;
        
        if (authLevelClaim != null)
        {
            var authLevels = new[] { "AAL1", "AAL2", "AAL3" };
            var currentLevelIndex = Array.IndexOf(authLevels, authLevelClaim);
            var requiredLevelIndex = Array.IndexOf(authLevels, requirement.MinAuthLevel);
            
            if (currentLevelIndex >= requiredLevelIndex)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}

/// <summary>
/// Authorization handler for identity provider
/// </summary>
public class IdentityProviderHandler : AuthorizationHandler<IdentityProviderRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IdentityProviderRequirement requirement)
    {
        var idpClaim = context.User.FindFirst("idp")?.Value;
        
        if (idpClaim == requirement.RequiredIdp)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

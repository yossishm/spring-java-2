using Microsoft.AspNetCore.Authorization;

namespace SpringJavaEquivalent.Authorization;

/// <summary>
/// Authorization requirement for permission-based access control
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; }

    public PermissionRequirement(string permission)
    {
        Permission = permission;
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
        Permissions = permissions;
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
        Permissions = permissions;
    }
}

/// <summary>
/// Authorization requirement for authentication level
/// </summary>
public class AuthLevelRequirement : IAuthorizationRequirement
{
    public string MinAuthLevel { get; }

    public AuthLevelRequirement(string minAuthLevel)
    {
        MinAuthLevel = minAuthLevel;
    }
}

/// <summary>
/// Authorization requirement for identity provider
/// </summary>
public class IdentityProviderRequirement : IAuthorizationRequirement
{
    public string RequiredIdp { get; }

    public IdentityProviderRequirement(string requiredIdp)
    {
        RequiredIdp = requiredIdp;
    }
}

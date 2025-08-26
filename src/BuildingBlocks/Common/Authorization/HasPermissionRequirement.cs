using Microsoft.AspNetCore.Authorization;

namespace Common.Authorization;

public class HasPermissionRequirement : IAuthorizationRequirement
{
    public string Permission { get; set; }

    public HasPermissionRequirement(string permission)
    {
        Permission = permission;
    }
}
using Common.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authorization;

public class PermissionAuthorizationHandler : AuthorizationHandler<HasPermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        HasPermissionRequirement requirement)
    {
        var userPermissions = context.User.FindAll("permissions");

        if (userPermissions.Any(p => p.Value == requirement.Permission))
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }   
}
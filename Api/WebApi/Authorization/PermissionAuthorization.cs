using Microsoft.AspNetCore.Authorization;
using WebApi.Domain.Accounts;

namespace WebApi.Authorization;

public class PermissionRequirement(PermissionTypes permission) : IAuthorizationRequirement
{
    public PermissionTypes Permission { get; } = permission;
}


public class PermissionHandler(ITokenAuthService tokenAuthService) : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var permissionClaim = tokenAuthService.GetPermissionClaims(context.User);
        var hasPermission = permissionClaim.Contains(requirement.Permission);
        if (hasPermission)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
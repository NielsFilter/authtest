using Microsoft.Identity.Client;
using WebApi.Data.Profile;
using WebApi.Helpers;

namespace WebApi.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApi.Entities;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly IList<Role> _roles;

    public AuthorizeAttribute(params Role[]? roles)
    {
        _roles = roles ?? [];
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // skip authorization if action is decorated with [AllowAnonymous] attribute
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
        {
            return;
        }

        // authorization
        var accountId = context.HttpContext.Items["AccountId"] as int?;
        if (accountId is <= 0)
        {
            // not logged in
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };   
        }

        if (!_roles.Any())
        {
            // no roles required for this action. User is authorized, let them in...
            return;
        }
        
        // This action requires a specific role. First check if the account has that role
        var accountRoles = context.HttpContext.Items["AccountRoles"] as List<Role>;
        if (accountRoles == null || !_roles.Any(r => accountRoles.Any(ar => ar == r)))
        {
            // user does not have the required role
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
        }
    }
}
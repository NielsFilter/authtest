using WebApi.Authorization;

namespace WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
public abstract class AuthenticatedController(ITokenAuthService tokenAuthService) : ControllerBase
{
    // returns the current authenticated account (null if not logged in)
    public int? AccountId => tokenAuthService.GetAccountId(HttpContext.User);
    public bool IsAccountAdmin => tokenAuthService.IsAccountAdmin(HttpContext.User);
}
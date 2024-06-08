namespace WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using WebApi.Entities;

[ApiController]
public abstract class BaseController : ControllerBase
{
    // returns the current authenticated account (null if not logged in)
    public int? AccountId => HttpContext.Items["AccountId"] as int?;

    // returns the current authenticated account (null if not logged in)
    public List<Role> AccountRoles => HttpContext.Items["AccountRoles"] as List<Role> ?? new();

    public bool IsAccountAdmin => AccountRoles.Contains(Role.Admin);
}
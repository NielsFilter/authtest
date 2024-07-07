using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;
using WebApi.Data.Profile;
using WebApi.Helpers;
using WebApi.Models.Accounts;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route($"{ApiVersioning.V1}/[controller]")]
public class ProfileController(IProfileService profileService) : BaseController
{

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AccountDto>> GetById(int id)
    {
        // users can get their own account and admins can get any account
        if (id != AccountId)
        {
            return Unauthorized(new { message = "Unauthorized" });
        }

        var account = await profileService.GetById(id);
        return Ok(account);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AccountDto>> Update(int id, ProfileUpdateRequest model)
    {
        // users can update their own account and admins can update any account
        if (id != AccountId)
        {
            return Unauthorized(new { message = "Unauthorized" });
        }

        //TODO: 
        // if (!IsAccountAdmin)
        // {
        //     // only admins can update role
        //     model.Roles = new List<Role>();
        // }

        var account = await profileService.Update(id, model);
        return Ok(account);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        // users can delete their own account and admins can delete any account
        if (id != AccountId)
        {
            return Forbid();
        }

        await profileService.Delete(id);
        return Ok(new { message = "Account deleted successfully" });
    }

    //TODO: 
    // private async Task<AccountDto> GetLoggedInAccountOrThrow()
    // {
    //     if (AccountId == null)
    //     {
    //         throw new AppException("Unauthorized"); //tODO: exceptions
    //     }
    //     
    //     var account = await profileService.GetById(AccountId.Value);
    //     if (account == null)
    //     {
    //         throw new AppException("Unauthorized"); //tODO: exceptions
    //     }
    //
    //     return account;
    // }
    
    
    [HttpGet("session-info")]
    public async Task<AccountSessionInfo> GetAccountSessionInfo()
    {
        var account = await profileService.GetById(AccountId!.Value);
        if (account == null)
        {
            throw new AppException("Unauthorized"); //tODO: exceptions
        }
        return new AccountSessionInfo(account);
    }
}
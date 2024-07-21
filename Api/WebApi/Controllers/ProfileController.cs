using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Accounts.Models;
using WebApi.Authorization;
using WebApi.Data.Accounts.Models;
using WebApi.Data.Profile;
using WebApi.Data.Settings.Models;
using WebApi.Helpers;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route($"{ApiVersioning.V1}/[controller]")]
public class ProfileController(
    ITokenAuthService tokenAuthService,
    IProfileService profileService)
    : AuthenticatedController(tokenAuthService)
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
    
    [HttpPut("personal")]
    public async Task<ActionResult<AccountDto>> UpdatePersonal(ProfilePersonalUpdateRequest input)
    {
        // users can get their own account and admins can get any account
        if (input.AccountId != AccountId)
        {
            return Unauthorized(new { message = "Unauthorized" });
        }
        
        var account = await profileService.UpdatePersonal(input);
        return Ok(account);
    }
    
    [HttpPut("security")]
    public async Task<ActionResult> UpdateSecurity(ProfileSecurityUpdateRequest input)
    {
        // users can get their own account and admins can get any account
        if (input.AccountId != AccountId)
        {
            return Unauthorized(new { message = "Unauthorized" });
        }
        
        await profileService.UpdateSecurity(input);
        return Ok();
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
    
    [HttpPut("dark-mode")]
    public async Task<ActionResult> UpdateDarkMode(bool isDarkMode)
    {
        var request = new UpdateDarkModeSettingRequest
        {
            AccountId = AccountId!.Value,
            IsDarkMode = isDarkMode
        };
        await profileService.SetDarkMode(request);
        return Ok();
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
    
    [HttpGet("settings")]
    public async Task<ProfileSettingResult> GetAccountSettings()
    {
        var profileSettings = await profileService.GetProfileSettings(AccountId!.Value);
        if (profileSettings == null)
        {
            throw new AppException("Unauthorized"); //tODO: exceptions
        }

        return profileSettings;
    }
}
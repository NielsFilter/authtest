using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Accounts.Models;
using WebApi.Authorization;
using WebApi.Data.Profile;
using WebApi.Entities;
using WebApi.Services;

namespace WebApi.Controllers;

[Authorize(Roles = nameof(Role.Admin))]
[ApiController]
[Route($"{ApiVersioning.V1Admin}/profile")]
public class AdminProfileController(
    ITokenAuthService tokenAuthService,
    IProfileService profileService)
    : AuthenticatedController(tokenAuthService)
{
    [HttpPost("search")]
    public async Task<ActionResult<IEnumerable<AccountDto>>> SearchAllPaged(FilterPagedDto input, CancellationToken cancellationToken = default)
    {
        var accounts = await profileService.ListAllPaged(input, cancellationToken);
        return Ok(accounts);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<AccountDto>> GetById(int id)
    {
        var account = await profileService.GetById(id);
        return Ok(account);
    }

    [HttpPost]
    public async Task<ActionResult<AccountDto>> Create(ProfileCreateRequest model)
    {
        var account = await profileService.Create(model);
        return Ok(account);
    }
    
    [HttpPut("{id:int}")]
    public async Task<ActionResult<AccountDto>> Update(int id, ProfileUpdateRequest model)
    {
        //TODO: Allow Admin to update roles
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
        if(id == AccountId)
        {
            return BadRequest(new { message = "You cannot delete your own admin account" });
        }
        
        // users can delete their own account and admins can delete any account
        await profileService.Delete(id);
        return Ok(new { message = "Account deleted successfully" });
    }
}
using WebApi.Helpers;

namespace WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Authorization;
using Entities;
using Models.Accounts;
using Services;

[Authorize]
[ApiController]
[Route($"{ApiVersioning.V1}/[controller]")]
public class AccountsController(IAccountService accountService) : BaseController
{
    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<ActionResult<AuthenticationResult>> Authenticate(AuthenticateRequest model)
    {
        var response = await accountService.Authenticate(model, GetIpAddress());
        //TODO: SetTokenCookie(response.RefreshToken);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthenticationResult>> RefreshToken(string refreshToken)
    {
        //TODO: var refreshToken = Request.Cookies["refreshToken"];
        var response = await accountService.RefreshToken(refreshToken, GetIpAddress());
       //TODO:  SetTokenCookie(response.RefreshToken);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest model)
    {
        await accountService.Register(model, Request.Headers["origin"]);
        return Ok(new { message = "Registration successful, please check your email for verification instructions" });
    }

    [AllowAnonymous]
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailRequest model)
    {
        await accountService.VerifyEmail(model.Token);
        return Ok(new { message = "Verification successful, you can now login" });
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
    {
        await accountService.ForgotPassword(model, Request.Headers["origin"]);
        return Ok(new { message = "Please check your email for password reset instructions" });
    }

    [AllowAnonymous]
    [HttpPost("validate-reset-token")]
    public async Task<IActionResult> ValidateResetToken(ValidateResetTokenRequest model)
    {
        await accountService.ValidateResetToken(model);
        return Ok(new { message = "Token is valid" });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
    {
        await accountService.ResetPassword(model);
        return Ok(new { message = "Password reset successful, you can now login" });
    }

    [Authorize(Role.Admin)]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AccountDto>>> GetAll()
    {
        var accounts = await accountService.GetAll();
        return Ok(accounts);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AccountDto>> GetById(int id)
    {
        // users can get their own account and admins can get any account
        if (id != AccountId && !IsAccountAdmin)
        {
            return Unauthorized(new { message = "Unauthorized" });
        }

        var account = await accountService.GetById(id);
        return Ok(account);
    }

    [Authorize(Role.Admin)]
    [HttpPost]
    public async Task<ActionResult<AccountDto>> Create(CreateRequest model)
    {
        var account = await accountService.Create(model);
        return Ok(account);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<AccountDto>> Update(int id, UpdateRequest model)
    {
        // users can update their own account and admins can update any account
        if (id != AccountId && !IsAccountAdmin)
        {
            return Unauthorized(new { message = "Unauthorized" });
        }

        //TODO: 
        // if (!IsAccountAdmin)
        // {
        //     // only admins can update role
        //     model.Roles = new List<Role>();
        // }

        var account = await accountService.Update(id, model);
        return Ok(account);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        // users can delete their own account and admins can delete any account
        if (id != AccountId && IsAccountAdmin)
            return Unauthorized(new { message = "Unauthorized" });

        await accountService.Delete(id);
        return Ok(new { message = "Account deleted successfully" });
    }

    // helper methods

    //TODO: 
    // private void SetTokenCookie(string token)
    // {
    //     var cookieOptions = new CookieOptions
    //     {
    //         HttpOnly = true,
    //         Domain = Request.Host.Value, //TODO: new Uri(Request.Headers.Origin!).Authority,
    //         Expires = DateTime.UtcNow.AddDays(7)
    //     };
    //     Response.Cookies.Append("refreshToken", token, cookieOptions);
    // }

    private string GetIpAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"];
        else
            return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
    }

    private async Task<AccountDto> GetLoggedInAccountOrThrow()
    {
        if (AccountId == null)
        {
            throw new AppException("Unauthorized"); //tODO: exceptions
        }
        
        var account = await accountService.GetById(AccountId.Value);
        if (account == null)
        {
            throw new AppException("Unauthorized"); //tODO: exceptions
        }

        return account;
    }
    
    [HttpGet("session-info")]
    public async Task<AccountSessionInfo> GetAccountSessionInfo()
    {
        var account = await accountService.GetById(AccountId!.Value);
        if (account == null)
        {
            throw new AppException("Unauthorized"); //tODO: exceptions
        }
        return new AccountSessionInfo(account);
    }
}
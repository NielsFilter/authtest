using Microsoft.AspNetCore.Mvc;
using WebApi.Authorization;
using WebApi.Helpers;
using WebApi.Models.Accounts;
using WebApi.Services;

namespace WebApi.Controllers;

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
}
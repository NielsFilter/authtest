using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Accounts.Models;
using WebApi.Authorization;
using WebApi.Services;

namespace WebApi.Controllers;

[Authorize]
[ApiController]
[Route($"{ApiVersioning.V1}/[controller]")]
public class AuthController(
    ITokenAuthService tokenAuthService,
    IAuthService authService)
    : AuthenticatedController(tokenAuthService)
{
    [AllowAnonymous]
    [HttpPost("authenticate")]
    public async Task<ActionResult<AuthenticationResult>> Authenticate(AuthenticateRequest model)
    {
        var response = await authService.Authenticate(model, GetIpAddress());
        //TODO: SetTokenCookie(response.RefreshToken);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public async Task<ActionResult<AuthenticationResult>> RefreshToken(string refreshToken)
    {
        //TODO: var refreshToken = Request.Cookies["refreshToken"];
        var response = await authService.RefreshToken(refreshToken, GetIpAddress());
       //TODO:  SetTokenCookie(response.RefreshToken);
        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest model)
    {
        await authService.Register(model, Request.Headers["origin"]);
        return Ok(new { message = "Registration successful, please check your email for verification instructions" });
    }

    [AllowAnonymous]
    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailRequest model)
    {
        await authService.VerifyEmail(model.Token);
        return Ok(new { message = "Verification successful, you can now login" });
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
    {
        await authService.ForgotPassword(model, Request.Headers["origin"]);
        return Ok(new { message = "Please check your email for password reset instructions" });
    }

    [AllowAnonymous]
    [HttpPost("validate-reset-token")]
    public async Task<IActionResult> ValidateResetToken(ValidateResetTokenRequest model)
    {
        await authService.ValidateResetToken(model);
        return Ok(new { message = "Token is valid" });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest model)
    {
        await authService.ResetPassword(model);
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
using WebApi.Shared;

namespace WebApi.Authorization;

using Microsoft.Extensions.Options;
using WebApi.Helpers;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly AppSettings _appSettings;

    public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
    {
        _next = next;
        _appSettings = appSettings.Value;
    }

    public async Task Invoke(HttpContext context, DataContext dataContext, ITokenAuthService jwtUtils)
    {
        const string bearerPrefix = "Bearer ";
        
        var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
        if(authHeader == null || !authHeader.StartsWith(bearerPrefix))
        {
            await _next(context);
            return;
        }
        
        // strip Bearer from authHeader
        var bearerToken = authHeader[bearerPrefix.Length..];
        var claim = jwtUtils.ValidateJwtToken(bearerToken);
        if (claim != null)
        {
            // attach account to context on successful jwt validation
            context.Items["AccountId"] = claim.AccountId;
            context.Items["AccountRoles"] = claim.Roles;
        }

        await _next(context);
    }
}
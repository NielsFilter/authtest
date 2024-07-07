//TODO: DELETE
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using WebApi.Entities;
// using WebApi.Shared;
//
// namespace WebApi.Authorization;
//
// using Microsoft.Extensions.Options;
// using Helpers;
//
// public class JwtMiddleware
// {
//     private readonly RequestDelegate _next;
//     private readonly AppSettings _appSettings;
//
//     public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> appSettings)
//     {
//         _next = next;
//         _appSettings = appSettings.Value;
//     }
//
//     public async Task Invoke(HttpContext context, DataContext dataContext, ITokenAuthService jwtUtils)
//     {
//         context.Items["AccountId"] = context.User.Claims.Id 
//         
//         // //TODO: 
//         // var endpoint = context.GetEndpoint();
//         // if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
//         // {
//         //     // skip access token checks, the action is decorated with [AllowAnonymous] attribute
//         //     await _next(context);
//         //     return;
//         // }
//         //
//         // const string bearerPrefix = "Bearer ";
//         //
//         // var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
//         // if (authHeader == null || !authHeader.StartsWith(bearerPrefix))
//         // {
//         //     await _next(context);
//         //     return;
//         // }
//         
//         await _next(context);
//     }
// }
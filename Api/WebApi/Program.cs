﻿using System.Reflection;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi;
using WebApi.Authorization;
using WebApi.Data.Profile;
using WebApi.Domain;
using WebApi.Domain.Accounts;
using WebApi.Helpers;
using WebApi.Infrastructure;
using WebApi.Services;
using WebApi.Shared;

var builder = WebApplication.CreateBuilder(args);

// add services to DI container
{
    var services = builder.Services;
    var env = builder.Environment;
 
    services.AddDbContext<DataContext>();
    services.AddCors();
    services.AddControllers().AddJsonOptions(x => 
    {
        // serialize enums as strings in api responses (e.g. Role)
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
    services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

    services.AddSwaggerGen(c =>
    {
        // We'll use the method name as the operation id
        c.CustomOperationIds(apiDesc =>
        {
            if (apiDesc.TryGetMethodInfo(out var methodInfo))
            {
                return methodInfo.DeclaringType?.Name != null ? $"{methodInfo.DeclaringType.Name.Replace("Controller", "")}{methodInfo.Name}" : methodInfo.Name;
            }

            return null;
        });
    });
    
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    builder.Services.AddAuthentication(opt => {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["securityKey"])),
                ValidateLifetime = true
            };
        });
    
    builder.Services.AddAuthorization(options =>
    {
        foreach(var permissionType in Enum.GetValues<PermissionTypes>()) 
        {
            // assuming .Permission is enum
            options.AddPolicy(permissionType.ToString(),
                policy => policy.Requirements.Add(new PermissionRequirement(permissionType)));
        }
    });

    // configure strongly typed settings object
    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    services.AddSignalR();
    services.AddHttpContextAccessor();
    
    // configure DI for application services
    DependencyInjection.AddServices(services);
}

var app = builder.Build();

// migrate any database changes on startup (includes initial db creation)
using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();    
    dataContext.Database.Migrate();
}

// configure HTTP request pipeline
{
    // generated swagger json and swagger ui middleware
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    
    var options = app.Services.GetRequiredService<IOptions<AppSettings>>();
    // global cors policy
    app.UseCors(x => x
        .WithOrigins(options.Value.AllowedOrigins.Split(";"))
        //TODO: .SetIsOriginAllowed(origin => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());

    // global error handler
    app.UseMiddleware<ErrorHandlerMiddleware>();
    
    app.UseAuthentication();
    app.UseAuthorization();

    
    app.MapHub<SignalrAppNotificationHub>("/Notify");
    app.MapControllers();
}

app.Run();
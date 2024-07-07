using WebApi.Authorization;
using WebApi.Data.Profile;
using WebApi.Helpers;
using WebApi.Infrastructure;
using WebApi.Services;

namespace WebApi;

public abstract class DependencyInjection
{
    public static void AddServices(IServiceCollection services)
    {
        services.AddScoped<IRepositoryFactory, RepositoryFactory>();
        
        services.AddScoped<ITokenAuthService, TokenAuthService>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProfileService, ProfileService>();
        
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationService, NotificationService>();
        
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAppNotifier, SignalrAppNotifier>();
        
        services.AddSingleton<ILoggedInUserResolver, LoggedInUserResolver>();
    }
}
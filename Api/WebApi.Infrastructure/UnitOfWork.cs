using WebApi.Domain;
using WebApi.Helpers;
using WebApi.Services;

namespace WebApi.Infrastructure;

public class UnitOfWork (
    IServiceProvider serviceProvider,
    DataContext context) : IUnitOfWork
{
    private INotificationRepository? _notificationRepository;
    public INotificationRepository NotificationRepository => _notificationRepository ??= new NotificationRepository(context);
    
    private IAccountRepository? _accountRepository;
    public IAccountRepository AccountRepository => _accountRepository ??= new AccountRepository(context);
    
    public async Task Commit()
    {
        await context.SaveChangesAsync();
    }

    public void Dispose()
    {
        context.Dispose();
    }
}
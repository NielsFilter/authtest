using WebApi.Domain;
using WebApi.Domain.Settings;
using WebApi.Helpers;
using WebApi.Services;

namespace WebApi.Infrastructure;

public class RepositoryFactory(DataContext context) : IRepositoryFactory
{
    public IUnitOfWork CreateUnitOfWork() => new UnitOfWork(context);
    public IAccountRepository CreateAccountRepository() => new AccountRepository(context);
    public INotificationRepository CreateNotificationRepository() => new NotificationRepository(context);
    public IProfileSettingRepository CreateSettingRepository() => new ProfileSettingRepository(context);
}
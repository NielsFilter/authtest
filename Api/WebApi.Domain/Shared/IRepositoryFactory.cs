using WebApi.Domain;
using WebApi.Domain.Settings;

namespace WebApi.Services;

public interface IRepositoryFactory
{
    IUnitOfWork CreateUnitOfWork(); 
    IAccountRepository CreateAccountRepository();
    INotificationRepository CreateNotificationRepository();
    IProfileSettingRepository CreateSettingRepository();
}
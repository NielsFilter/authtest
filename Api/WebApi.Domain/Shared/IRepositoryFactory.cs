using WebApi.Domain;

namespace WebApi.Services;

public interface IRepositoryFactory
{
    IUnitOfWork CreateUnitOfWork(); 
    IAccountRepository CreateAccountRepository();
    INotificationRepository CreateNotificationRepository();
}
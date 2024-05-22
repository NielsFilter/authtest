using WebApi.Services;

namespace WebApi.Domain;

public interface IUnitOfWork : IDisposable
{
    INotificationRepository NotificationRepository { get; }
    IAccountRepository AccountRepository { get; }
    Task Commit();
}
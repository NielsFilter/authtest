using WebApi.Accounts.Models;

namespace WebApi.Services;

public interface INotificationHub
{
    public Task NewNotification(NewAccountAppNotification newAccountAppNotification);
}
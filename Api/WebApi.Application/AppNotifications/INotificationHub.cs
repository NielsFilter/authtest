using WebApi.Entities;
using WebApi.Models.Accounts;

namespace WebApi.Services;

public interface INotificationHub
{
    public Task NewNotification(NewAccountAppNotification newAccountAppNotification);
}
using WebApi.Accounts.Models;

namespace WebApi.Helpers;

public interface IAppNotifier
{
    public Task NewAccountNotification(NewAccountAppNotification newAccountAppNotification);
}
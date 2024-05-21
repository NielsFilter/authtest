using WebApi.Models.Accounts;

namespace WebApi.Helpers;

public interface IAppNotifier
{
    public Task NewAccountNotification(NewAccountAppNotification newAccountAppNotification);
}
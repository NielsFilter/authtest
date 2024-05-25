using Microsoft.AspNetCore.SignalR;
using WebApi.Models.Accounts;
using WebApi.Services;

namespace WebApi.Helpers;

public class SignalrAppNotifier(IHubContext<SignalrAppNotificationHub, IAppNotifier> hubContext)
    : Hub, IAppNotifier
{
    public async Task NewAccountNotification( NewAccountAppNotification newAccountAppNotification)
    {
        var client = hubContext.Clients.User(newAccountAppNotification.TargetAccountId.ToString());
        await client.NewAccountNotification(newAccountAppNotification);  
    }
}
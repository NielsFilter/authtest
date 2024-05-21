using Microsoft.AspNetCore.SignalR;
using WebApi.Helpers;

namespace WebApi.Services;

public class SignalrAppNotificationHub : Hub<IAppNotifier>
{
}
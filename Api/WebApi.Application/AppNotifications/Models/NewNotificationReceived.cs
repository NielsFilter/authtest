using WebApi.Entities;

namespace WebApi.Accounts.Models;

public class NewNotificationReceived
{
    public string Message { get; set; }
    public NotificationTypes Type { get; set; }
}
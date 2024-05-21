using WebApi.Entities;

namespace WebApi.Models.Accounts;

public class NewNotificationReceived
{
    public string Message { get; set; }
    public NotificationTypes Type { get; set; }
}
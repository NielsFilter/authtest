using WebApi.Entities;

namespace WebApi.Accounts.Models;

public class NewNotificationRequest
{
    public string Message { get; set; }
    public NotificationTypes Type { get; set; }
    public int TargetAccountId { get; set; }
}
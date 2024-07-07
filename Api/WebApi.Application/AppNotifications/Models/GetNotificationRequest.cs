namespace WebApi.Accounts.Models;

public class GetNotificationRequest
{
    public int AccountId { get; set; }
    public int NotificationId { get; set; }
}
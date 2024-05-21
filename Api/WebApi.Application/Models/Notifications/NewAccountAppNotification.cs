using WebApi.Entities;

namespace WebApi.Models.Accounts;

public class NewAccountAppNotification
{
    public int Id { get; set; }
    public string Message { get; set; }
    public NotificationTypes Type { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedTimestamp { get; set; }
    public int TargetAccountId { get; set; }
}
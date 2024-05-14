using WebApi.Entities;

namespace WebApi.Models.Accounts;

public class NotificationResponse
{
    public int Id { get; set; }
    public string Message { get; set; }
    public NotificationTypes Type { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedTimestamp { get; set; }
}
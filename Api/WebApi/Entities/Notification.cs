using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities;

public class Notification : FullAuditedEntity
{
    public string Message { get; set; }
    public NotificationTypes Type { get; set; }
    [ForeignKey(nameof(TargetAccount))]
    public int TargetAccountId { get; set; }
    public Account TargetAccount { get; set; }
    public bool IsRead { get; set; }
}
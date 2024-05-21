using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities;

public class Notification : FullAuditedEntity
{
    [MaxLength(500)]
    public string Message { get; set; }
    public NotificationTypes Type { get; set; }
    public int TargetAccountId { get; set; }
    public Account TargetAccount { get; set; }
    public bool IsRead { get; set; }
}
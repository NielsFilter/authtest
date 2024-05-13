using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities;

public class FullAuditedEntity : EntityBase, IFullAudit
{
    public DateTime CreatedTimestamp { get; set; }
    [ForeignKey(nameof(CreatedUser))]
    public int CreatedUserId { get; set; }
    public Account CreatedUser { get; set; }
    [ForeignKey(nameof(DeletedByUser))]
    public int? DeletedByUserId { get; set; }
    public Account? DeletedByUser { get; set; }
    public DateTime? DeletedTimestamp { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? UpdatedTimestamp { get; set; }
    [ForeignKey(nameof(UpdatedUser))]
    public int? UpdatedUserId { get; set; }
    public Account? UpdatedUser { get; set; }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities;

public class FullAuditedEntity : EntityBase, IFullAudit
{
    public DateTime CreatedTimestamp { get; set; }
    [ForeignKey(nameof(CreatedBy))]
    public int CreatedById { get; set; }
    public Account CreatedBy { get; set; }
    [ForeignKey(nameof(DeletedBy))]
    public int? DeletedById { get; set; }
    public Account? DeletedBy { get; set; }
    public DateTime? DeletedTimestamp { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? UpdatedTimestamp { get; set; }
    [ForeignKey(nameof(UpdatedBy))]
    public int? UpdatedById { get; set; }
    public Account? UpdatedBy { get; set; }
}
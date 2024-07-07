namespace WebApi.Entities;

public interface IDeleteAudit
{
    public int? DeletedById { get; set; }
    public DateTime? DeletedTimestamp { get; set; }
    bool IsDeleted { get; set; }
}
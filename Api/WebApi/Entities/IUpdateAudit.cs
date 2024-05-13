namespace WebApi.Entities;

public interface IUpdateAudit
{ 
    DateTime? UpdatedTimestamp { get; set; }
    public int? UpdatedUserId { get; set; }
}
namespace WebApi.Entities;

public interface ICreateAudit
{ 
    DateTime CreatedTimestamp { get; set; }
    public int CreatedUserId { get; set; }
}
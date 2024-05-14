namespace WebApi.Entities;

public interface ICreateAudit
{ 
    DateTime CreatedTimestamp { get; set; }
    public int CreatedById { get; set; }
}
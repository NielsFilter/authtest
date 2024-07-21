using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities;

public class Entity
{
    [Key]
    public virtual int Id { get; set; }
}
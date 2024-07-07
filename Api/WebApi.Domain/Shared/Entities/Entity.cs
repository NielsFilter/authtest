using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities;

public class Entity
{
    [Key]
    public int Id { get; set; }
}
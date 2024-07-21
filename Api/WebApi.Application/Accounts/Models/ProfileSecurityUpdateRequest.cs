using System.Runtime.InteropServices;

namespace WebApi.Accounts.Models;

using System.ComponentModel.DataAnnotations;
using WebApi.Entities;

public class ProfileSecurityUpdateRequest
{
    public int AccountId { get; set; }
    [Required]
    [MinLength(6)]
    public string? Password { get; set; }
}
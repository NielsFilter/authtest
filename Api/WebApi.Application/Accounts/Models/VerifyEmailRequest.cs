namespace WebApi.Accounts.Models;

using System.ComponentModel.DataAnnotations;

public class VerifyEmailRequest
{
    [Required]
    public string Token { get; set; }
}
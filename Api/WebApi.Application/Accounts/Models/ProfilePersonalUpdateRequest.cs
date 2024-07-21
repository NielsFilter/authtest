namespace WebApi.Accounts.Models;

using System.ComponentModel.DataAnnotations;

public class ProfilePersonalUpdateRequest
{
    public int AccountId { get; set; }
    public string Title { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string? Image { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

}
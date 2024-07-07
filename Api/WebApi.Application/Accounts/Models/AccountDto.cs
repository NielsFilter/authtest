namespace WebApi.Accounts.Models;

public class AccountDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public bool IsVerified { get; set; }
}
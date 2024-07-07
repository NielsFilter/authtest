namespace WebApi.Accounts.Models;

public class AuthenticationResult
{
    public  AccountDto Account { get; set; }   
    public AuthenticateDto Authenticate { get; set; }
}
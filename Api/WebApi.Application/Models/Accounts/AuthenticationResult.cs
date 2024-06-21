namespace WebApi.Models.Accounts;

public class AuthenticationResult
{
    public  AccountDto Account { get; set; }   
    public AuthenticateDto Authenticate { get; set; }
}
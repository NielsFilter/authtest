namespace WebApi.Accounts.Models;

public class AccountSessionInfo(AccountDto account)
{
    public AccountDto Account { get; set; } = account;
}
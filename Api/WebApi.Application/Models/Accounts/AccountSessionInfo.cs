namespace WebApi.Models.Accounts;

public class AccountSessionInfo(AccountDto account)
{
    public AccountDto Account { get; set; } = account;
}
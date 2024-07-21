namespace WebApi.Data.Accounts.Models;

public class UpdateDarkModeSettingRequest
{
    public int AccountId { get; set; }
    public bool IsDarkMode { get; set; }
}
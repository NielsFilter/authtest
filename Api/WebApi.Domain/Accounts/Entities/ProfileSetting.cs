using WebApi.Entities;

namespace WebApi.Domain.Settings;

public class ProfileSetting : Entity
{
    public Account Account { get; set; }
    public bool IsDarkMode { get; set; }
}
using WebApi.Domain.Settings;
using WebApi.Helpers;

namespace WebApi.Infrastructure;

public class ProfileSettingRepository(DataContext context)
    : BaseRepository<ProfileSetting>(context), IProfileSettingRepository
{
}
using WebApi.Entities;

namespace WebApi.Data.Profile;

public interface ILoggedInUserResolver
{
    Account? GetLoggedInAccount();
}
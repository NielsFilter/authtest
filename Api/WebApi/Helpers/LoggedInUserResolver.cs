using WebApi.Data.Profile;
using WebApi.Entities;

namespace WebApi.Helpers;

public class LoggedInUserResolver(IHttpContextAccessor context) : ILoggedInUserResolver
{
    public Account? GetLoggedInAccount()
    {
        return context.HttpContext?.Items["Account"] as Account;
    }
}
using WebApi.Entities;

namespace WebApi.Services;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<Account?> GetByEmail(string email);
}
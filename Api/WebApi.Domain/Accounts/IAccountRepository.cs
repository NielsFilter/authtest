using WebApi.Domain.Profile;
using WebApi.Entities;

namespace WebApi.Services;

public interface IAccountRepository : IRepository<Account>
{
    Task<Account?> GetByEmail(string email);
    Task AddNewRefreshToken(int accountId, RefreshToken refreshToken);
    Task RemoveRefreshTokensOlderThanTtl(int accountId, int ttlDays);
    Task<Account?> GetByRefreshToken(string token);
    Task<Account?> GetByResetToken(string token);
    Task<Account?> GetByVerificationToken(string token);
    Task RevokeToken(int accountId, string token, string ipAddress, string revokeReason);
    Task SetRoles(int accountId, List<Role> roles);
    Task<List<Role>> GetAccountRoles(int accountId);
    Task<List<Account>> SearchPaged(FilterPagedDto input, CancellationToken cancellationToken = default);
}
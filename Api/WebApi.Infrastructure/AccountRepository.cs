using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Services;

namespace WebApi.Helpers;

public class AccountRepository(DataContext context)
    : BaseRepository<Account>(context), IAccountRepository
{
    private readonly DataContext _dbContext = context;
    
    public async Task<Account?> GetByEmail(string email)
    {
        return await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task AddNewRefreshToken(int accountId, RefreshToken refreshToken)
    {
        var account = await _dbContext.Accounts.FindAsync(accountId);
        if(account == null) throw new Exception("Account not found"); //tODO: VALIDATION EXCEPTIONS
            
        account.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task RemoveRefreshTokensOlderThanTtl(int accountId, int ttlDays)
    {
        if (ttlDays == 0)
        {
            //tODO: VALIDATION EXCEPTIONS
            throw new Exception("TTL must be greater than 0");
        }
        
        var account = await _dbContext.Accounts.FindAsync(accountId);
        if(account == null) throw new Exception("Account not found"); //tODO: VALIDATION EXCEPTIONS

        account.RefreshTokens.RemoveAll(x => 
            !x.IsActive && 
            x.Created.AddDays(ttlDays) <= DateTime.UtcNow);
    }

    public Task<Account?> GetByRefreshToken(string token)
    {
        return _dbContext.Accounts
            .Include(x=>x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.RefreshTokens.Any(t => t.Token == token));
    }
    
    public Task<Account?> GetByResetToken(string token)
    {
        return _dbContext.Accounts
            .Include(x=>x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.ResetToken == token);
    }

    public Task<Account?> GetByVerificationToken(string token)
    {
        return _dbContext.Accounts
            .Include(x=>x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.VerificationToken == token);
    }

    public async Task RevokeToken(int accountId, string token, string ipAddress, string revokeReason)
    {
        var account = await GetById(accountId);
        if(account == null) throw new Exception("Account not found"); //tODO: VALIDATION EXCEPTIONS

        var refreshToken = account.RefreshTokens.FirstOrDefault(x => x.Token == token);
        if(refreshToken == null) throw new Exception("Refresh token not found"); //tODO: VALIDATION EXCEPTIONS
        refreshToken.Revoked = DateTime.UtcNow;
        refreshToken.RevokedByIp = ipAddress;
        refreshToken.ReasonRevoked = revokeReason;
        
        await _dbContext.SaveChangesAsync();
    }
}
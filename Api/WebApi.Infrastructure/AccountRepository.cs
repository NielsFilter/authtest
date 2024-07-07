using Microsoft.EntityFrameworkCore;
using WebApi.Domain.Profile;
using WebApi.Entities;
using WebApi.Infrastructure;
using WebApi.Services;

namespace WebApi.Helpers;

public class AccountRepository(DataContext context)
    : BaseRepository<Account>(context), IAccountRepository
{
    private readonly DataContext _context = context;

    public async Task<Account?> GetByEmail(string email)
    {
        return await _context.Accounts.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task AddNewRefreshToken(int accountId, RefreshToken refreshToken)
    {
        var account = await _context.Accounts.FindAsync(accountId);
        if(account == null) throw new Exception("Account not found"); //tODO: VALIDATION EXCEPTIONS
            
        account.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();
    }
    
    public async Task RemoveRefreshTokensOlderThanTtl(int accountId, int ttlDays)
    {
        if (ttlDays == 0)
        {
            //tODO: VALIDATION EXCEPTIONS
            throw new Exception("TTL must be greater than 0");
        }
        
        var account = await _context.Accounts.FindAsync(accountId);
        if(account == null) throw new Exception("Account not found"); //tODO: VALIDATION EXCEPTIONS

        account.RefreshTokens.RemoveAll(x => 
            !x.IsActive && 
            x.Created.AddDays(ttlDays) <= DateTime.UtcNow);
        await _context.SaveChangesAsync();
    }

    public Task<Account?> GetByRefreshToken(string token)
    {
        return _context.Accounts
            .Include(x=>x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.RefreshTokens.Any(t => t.Token == token));
    }
    
    public Task<Account?> GetByResetToken(string token)
    {
        return _context.Accounts
            .Include(x=>x.RefreshTokens)
            .FirstOrDefaultAsync(x => x.ResetToken == token);
    }

    public Task<Account?> GetByVerificationToken(string token)
    {
        return _context.Accounts
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
        
        await _context.SaveChangesAsync();
    }
    
    public async Task SetRoles(int accountId, List<Role> roles)
    {
        var existingAccountRoles = await _context.AccountRoles
            .Where(r => r.AccountId == accountId)
            .Select(r => r.Role)
            .ToListAsync();

        var rolesToAdd = roles
            .Where(r => !existingAccountRoles.Contains(r))
            .Select(r => new AccountRole
            {
                AccountId = accountId,
                Role = r
            });
        _context.AccountRoles.AddRange(rolesToAdd);
        
        var accountRolesToRemove = await _context.AccountRoles
            .Where(r => r.AccountId == accountId && !roles.Contains(r.Role))
            .ToListAsync();

        if (accountRolesToRemove.Any())
        {
            _context.AccountRoles.RemoveRange(accountRolesToRemove);
        }
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<Role>> GetAccountRoles(int accountId)
    {
        return await _context.AccountRoles
            .Where(x => x.AccountId == accountId)
            .Select(x => x.Role)
            .ToListAsync();
    }

    public async Task<List<Account>> SearchPaged(FilterPagedDto input, CancellationToken cancellationToken = default)
    {
        var sortBy = string.IsNullOrWhiteSpace(input.SortBy) ? nameof(Account.LastName) : input.SortBy;
        
        return await _context.Accounts
            .WhereIf(!string.IsNullOrEmpty(input.Search), x =>
                x.Email.Contains(input.Search!)
                || (x.FirstName + " " + x.LastName).Contains(input.Search!))
            .OrderBy(x => sortBy)
            .Skip(input.Index)
            .Take(input.PageSize)
            .ToListAsync(cancellationToken);
    }
}
namespace WebApi.Authorization;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Entities;
using Services;
using Shared;
using WebApi.Domain.Accounts;

public interface ITokenAuthService
{
    string GenerateJwtToken(Account account, List<Role> roles, List<PermissionTypes> permissions);
    Task<RefreshToken> GenerateRefreshToken(string ipAddress);
    List<PermissionTypes> GetPermissionClaims(ClaimsPrincipal user);
    int? GetAccountId(ClaimsPrincipal user);
    bool IsAccountAdmin(ClaimsPrincipal user);
}

public class TokenAuthService(
    IAccountRepository accountRepository,
    IOptions<AppSettings> appSettings,
    ILogger<TokenAuthService> logger) : ITokenAuthService
{
    private const string TokenClaimAccountId = "accountId";
    private const string TokenClaimAccountPermissions = "accountPermissions";
    
    private readonly AppSettings _appSettings = appSettings.Value;

    public string GenerateJwtToken(Account account, List<Role> roles, List<PermissionTypes> permissions)
    {
        var claims = CreateAuthClaims(account, roles, permissions);

        // generate token that is valid for 15 minutes
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddSeconds(20), //TODO: .AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static List<Claim> CreateAuthClaims(Account account, List<Role> roles, List<PermissionTypes> permissions)
    {
        var claims = new List<Claim>
        {
            new(TokenClaimAccountId, account.Id.ToString()),
            //TODO: Consider multiple permissions instead of string concatenated list (like with roles below)
            new(TokenClaimAccountPermissions, string.Join(",", permissions)),
        };

        foreach(var role in roles) 
        {
            claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
        }

        return claims;
    }

    public async Task<RefreshToken> GenerateRefreshToken(string ipAddress)
    {
        var refreshToken = new RefreshToken
        {
            // token is a cryptographically strong random sequence of values
            Token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64)),
            // token is valid for 7 days
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
            CreatedByIp = ipAddress
        };

        // ensure token is unique by checking against db
        var accountByToken = await accountRepository.GetByRefreshToken(refreshToken.Token);
        if (accountByToken != null)
        {
            return await GenerateRefreshToken(ipAddress);
        }
        
        return refreshToken;
    }
    
    public int? GetAccountId(ClaimsPrincipal user)
    {
        string? accountIdStr = user.Claims.FirstOrDefault(c => c.Type == TokenClaimAccountId)?.Value;
        if (string.IsNullOrWhiteSpace(accountIdStr) || !int.TryParse(accountIdStr, out var accountId))
        {
            return null;
        }

        return accountId;
    }
    
    public bool IsAccountAdmin(ClaimsPrincipal user)
    {
        var isAdmin = user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == Role.Admin.ToString());
        return isAdmin;
    }

    public List<PermissionTypes> GetPermissionClaims(ClaimsPrincipal user)
    {
        string? permissionsStr = null;
        try
        {
            permissionsStr = user.Claims.FirstOrDefault(c => c.Type == TokenClaimAccountPermissions)?.Value;
            if (string.IsNullOrWhiteSpace(permissionsStr))
            {
                return new List<PermissionTypes>();
            }

            return permissionsStr
                .Split(",")
                .Select(Enum.Parse<PermissionTypes>)
                .ToList();
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to parse permission claims '{permissionsStr}' from JWT token", e);
            return new List<PermissionTypes>();
        }
    }
}
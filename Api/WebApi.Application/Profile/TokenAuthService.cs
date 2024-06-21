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

public record TokenClaim(int AccountId, List<Role> Roles);

public interface ITokenAuthService
{
    string GenerateJwtToken(Account account, List<Role> roles);
    TokenClaim? ValidateJwtToken(string? token);
    Task<RefreshToken> GenerateRefreshToken(string ipAddress);
}

public class TokenAuthService(
    IAccountRepository accountRepository,
    IOptions<AppSettings> appSettings,
    ILogger<TokenAuthService> logger) : ITokenAuthService
{
    private const string TokenClaimAccountId = "accountId";
    private const string TokenClaimAccountRoles = "accountRoles";
    
    private readonly AppSettings _appSettings = appSettings.Value;

    public string GenerateJwtToken(Account account, List<Role> roles)
    {
        // generate token that is valid for 15 minutes
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(TokenClaimAccountId, account.Id.ToString()),
                new Claim(TokenClaimAccountRoles, string.Join(",", roles)),
            }),
            Expires = DateTime.UtcNow.AddSeconds(20), //TODO: .AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public TokenClaim? ValidateJwtToken(string? token)
    {
        if (token == null)
        {
            return null;
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var claim = ReadTokenClaims(validatedToken);
            // return account id from JWT token if validation successful
            return claim ?? null;
        }
        catch
        {
            // return null if validation fails
            return null;
        }
    }

    /// <summary>
    /// Read the claims from the JWT token and return parsed response
    /// </summary>
    private TokenClaim? ReadTokenClaims(SecurityToken validatedToken)
    {
        if(validatedToken is not JwtSecurityToken jwtToken)
        {
            logger.LogError("Security token is not a valid Jwt security token");
            return null;
        }
        
        var accountIdClaimStr = jwtToken.Claims.FirstOrDefault(x => x.Type == TokenClaimAccountId)?.Value;
        if (accountIdClaimStr == null)
        {
            logger.LogError($"No '{TokenClaimAccountId}' claim found in Jwt token");
            return null;
        }

        if (!int.TryParse(accountIdClaimStr, out var accountId) || accountId <= 0)
        {
            logger.LogError($"Invalid '{TokenClaimAccountId}' claim in Jwt token");    
        }

        var accountRolesClaimStr = jwtToken.Claims.FirstOrDefault(x => x.Type == TokenClaimAccountRoles)?.Value;
        if(string.IsNullOrWhiteSpace(accountRolesClaimStr))
        {
            logger.LogError($"No '{TokenClaimAccountRoles}' claim found in Jwt token");
            return null;
        }
        
        var rolesSplit = accountRolesClaimStr.Split(',');
        var roles = new List<Role>();
        foreach (var role in rolesSplit)
        {
            if (Enum.TryParse<Role>(role, out var parsedRole))
            {
                roles.Add(parsedRole);
            }
            else
            {
                logger.LogError($"'{role}' is an invalid '{TokenClaimAccountRoles}' claim in Jwt token");    
                return null;
            }
        }

        return new TokenClaim(accountId, roles);
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
}
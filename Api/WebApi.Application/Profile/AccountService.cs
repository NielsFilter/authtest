using AutoMapper;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using WebApi.Authorization;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Accounts;
using WebApi.Shared;

namespace WebApi.Services;
public interface IAccountService
{
    Task<AuthenticationResult> Authenticate(AuthenticateRequest model, string ipAddress);
    Task<AuthenticationResult> RefreshToken(string token, string ipAddress);
    Task Register(RegisterRequest model, string origin);
    Task VerifyEmail(string token);
    Task ForgotPassword(ForgotPasswordRequest model, string origin);
    Task ValidateResetToken(ValidateResetTokenRequest model);
    Task ResetPassword(ResetPasswordRequest model);
}

public class AccountService(
    IRepositoryFactory repositoryFactory,
    ITokenAuthService tokenAuthService,
    IMapper mapper,
    IOptions<AppSettings> appSettings,
    IEmailService emailService)
    : IAccountService
{
    private readonly AppSettings _appSettings = appSettings.Value;
    private readonly IAccountRepository _accountRepository = repositoryFactory.CreateAccountRepository();

    public async Task<AuthenticationResult> Authenticate(AuthenticateRequest model, string ipAddress)
    {
        var account = await _accountRepository.GetByEmail(model.Email);

        // validate
        if (account == null || !account.IsVerified || !BCrypt.Net.BCrypt.Verify(model.Password, account.PasswordHash))
        {
            throw new AppException("Email or password is incorrect");
        }
        
        // fetch the account roles
        var roles = await _accountRepository.GetAccountRoles(account.Id);

        // authentication successful so generate jwt and refresh tokens
        var jwtToken = tokenAuthService.GenerateJwtToken(account, roles);
        var refreshToken = await tokenAuthService.GenerateRefreshToken(ipAddress);
        
        var uow = repositoryFactory.CreateUnitOfWork();
        await uow.Run(async () =>
        {
            await _accountRepository.AddNewRefreshToken(account.Id, refreshToken);

            // remove old refresh tokens from account
            await _accountRepository.RemoveRefreshTokensOlderThanTtl(account.Id, _appSettings.RefreshTokenTTL);
        });

        return CreateAuthenticationResult(account, roles, jwtToken, refreshToken);
    }

    private AuthenticationResult CreateAuthenticationResult(Account account, List<Role> roles, string jwtToken,
        RefreshToken refreshToken)
    {
        var accountDto = mapper.Map<AccountDto>(account);
        accountDto.Roles = roles.Select(x => x.ToString()).ToList();
        return new AuthenticationResult
        {
            Account = accountDto,
            Authenticate = new AuthenticateDto
            {
                JwtToken = jwtToken,
                RefreshToken = refreshToken.Token
            }
        };
    }

    public async Task<AuthenticationResult> RefreshToken(string token, string ipAddress)
    {
        var uow = repositoryFactory.CreateUnitOfWork();
        return await uow.Run(async () =>
        {
            var account = await GetAccountByRefreshToken(token);
            var refreshToken = account.RefreshTokens.Single(x => x.Token == token);
        
            if (refreshToken.IsRevoked)
            {
                await _accountRepository.RevokeToken(account.Id, refreshToken.Token, ipAddress,
                    $"Attempted reuse of revoked ancestor token: {token}");
            }

            if (!refreshToken.IsActive)
            {
                throw new UnauthorizedAccessException("Invalid token");
            }

            // replace old refresh token with a new one (rotate token)
            var newRefreshToken = await tokenAuthService.GenerateRefreshToken(ipAddress);
            await _accountRepository.RevokeToken(account.Id, token, ipAddress, "Replaced by new token");
            await _accountRepository.AddNewRefreshToken(account.Id, newRefreshToken);

            // remove old refresh tokens from account
            await _accountRepository.RemoveRefreshTokensOlderThanTtl(account.Id, _appSettings.RefreshTokenTTL);
            
            // generate new jwt
            var roles = await _accountRepository.GetAccountRoles(account.Id);
            var jwtToken = tokenAuthService.GenerateJwtToken(account, roles);

            // return data in authenticate response object
            return CreateAuthenticationResult(account, roles, jwtToken, newRefreshToken);
        });
    }

    public async Task Register(RegisterRequest model, string origin)
    {
        // validate
        var existingAccount = await _accountRepository.GetByEmail(model.Email);
        if(existingAccount != null)
        {
            // send already registered error in email to prevent account enumeration
            SendAlreadyRegisteredEmail(model.Email, origin);
            return;
        }

        // map model to new account object
        var account = mapper.Map<Account>(model);

        // first registered account is an admin - TODO: THIS SHOULD LATER BE REPLACED...
        var isFirstAccount = !await _accountRepository.HasAny();
        
        account.CreatedDateTime = DateTime.UtcNow;
        account.VerificationToken = await GenerateVerificationToken();

        // hash password
        account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        // save account
        var uow = repositoryFactory.CreateUnitOfWork();
        await uow.Run(async () =>
        {
            await _accountRepository.Insert(account);
            
            var role = isFirstAccount ? Role.Admin : Role.User;
            await _accountRepository.SetRoles(account.Id, [role]);
        });
        
        // send email
        SendVerificationEmail(account, origin);
    }

    public async Task VerifyEmail(string token)
    {
        var account = await _accountRepository.GetByVerificationToken(token);

        if (account == null) 
            throw new AppException("Verification failed");

        account.Verified = DateTime.UtcNow;
        account.VerificationToken = null;

        await _accountRepository.Update(account);
    }

    public async Task ForgotPassword(ForgotPasswordRequest model, string origin)
    {
        var account = await  _accountRepository.GetByEmail(model.Email);

        // always return ok response to prevent email enumeration
        if (account == null) return;

        // create reset token that expires after 1 day
        account.ResetToken = await GenerateResetToken();
        account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

        await _accountRepository.Update(account);

        // send email
        SendPasswordResetEmail(account, origin);
    }

    public async Task ValidateResetToken(ValidateResetTokenRequest model)
    {
        await GetAccountByValidResetToken(model.Token);
    }

    public async Task ResetPassword(ResetPasswordRequest model)
    {
        var account = await GetAccountByValidResetToken(model.Token);

        // update password and remove reset token
        account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
        account.PasswordReset = DateTime.UtcNow;
        account.ResetToken = null;
        account.ResetTokenExpires = null;

        await _accountRepository.Update(account);
    }

    // helper methods

    private async Task<Account> GetAccountByRefreshToken(string token)
    {
        var account =  await  _accountRepository.GetByRefreshToken(token);
        if (account == null) throw new UnauthorizedAccessException("Invalid token");
        return account;
    }

    private async Task<Account> GetAccountByValidResetToken(string token)
    {
        var account = await  _accountRepository.GetByResetToken(token);
        if (account == null || account.ResetTokenExpires > DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid token");
        }

        return account;
    }

    private async Task<string> GenerateResetToken()
    {
        // token is a cryptographically strong random sequence of values
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        // ensure token is unique by checking against db
        var accountByResetToken = await  _accountRepository.GetByResetToken(token);
        if (accountByResetToken != null)
            return await GenerateResetToken();
        
        return token;
    }

    private async Task<string> GenerateVerificationToken()
    {
        // token is a cryptographically strong random sequence of values
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        // ensure token is unique by checking against db
        var accountByToken = await  _accountRepository.GetByVerificationToken(token);
        var tokenIsUnique = accountByToken == null;
        if (!tokenIsUnique)
        {
            return await GenerateVerificationToken();
        }

        return token;
    }
    
    private void SendVerificationEmail(Account account, string origin)
    {
        string message;
        if (!string.IsNullOrEmpty(origin))
        {
            // origin exists if request sent from browser single page app (e.g. Angular or React)
            // so send link to verify via single page app
            var verifyUrl = $"{origin}/account/verify-email?token={account.VerificationToken}";
            message = $@"<p>Please click the below link to verify your email address:</p>
                            <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";
        }
        else
        {
            // origin missing if request sent directly to api (e.g. from Postman)
            // so send instructions to verify directly with api
            message = $@"<p>Please use the below token to verify your email address with the <code>/accounts/verify-email</code> api route:</p>
                            <p><code>{account.VerificationToken}</code></p>";
        }

        emailService.Send(
            to: account.Email,
            subject: "Sign-up Verification API - Verify Email",
            html: $@"<h4>Verify Email</h4>
                        <p>Thanks for registering!</p>
                        {message}"
        );
    }

    private void SendAlreadyRegisteredEmail(string email, string origin)
    {
        string message;
        if (!string.IsNullOrEmpty(origin))
            message = $@"<p>If you don't know your password please visit the <a href=""{origin}/account/forgot-password"">forgot password</a> page.</p>";
        else
            message = "<p>If you don't know your password you can reset it via the <code>/accounts/forgot-password</code> api route.</p>";

        emailService.Send(
            to: email,
            subject: "Sign-up Verification API - Email Already Registered",
            html: $@"<h4>Email Already Registered</h4>
                        <p>Your email <strong>{email}</strong> is already registered.</p>
                        {message}"
        );
    }

    private void SendPasswordResetEmail(Account account, string origin)
    {
        string message;
        if (!string.IsNullOrEmpty(origin))
        {
            var resetUrl = $"{origin}/account/reset-password?token={account.ResetToken}";
            message = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                            <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
        }
        else
        {
            message = $@"<p>Please use the below token to reset your password with the <code>/accounts/reset-password</code> api route:</p>
                            <p><code>{account.ResetToken}</code></p>";
        }

        emailService.Send(
            to: account.Email,
            subject: "Sign-up Verification API - Reset Password",
            html: $@"<h4>Reset Password Email</h4>
                        {message}"
        );
    }
}
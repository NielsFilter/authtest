namespace WebApi.Services;

using Domain;
using Shared;
using AutoMapper;
using BCrypt.Net;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Authorization;
using Entities;
using Helpers;
using Models.Accounts;

public interface IAccountService
{
    Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress);
    Task<AuthenticateResponse> RefreshToken(string token, string ipAddress);
    Task RevokeToken(string token, string ipAddress);
    Task Register(RegisterRequest model, string origin);
    Task VerifyEmail(string token);
    Task ForgotPassword(ForgotPasswordRequest model, string origin);
    Task ValidateResetToken(ValidateResetTokenRequest model);
    Task ResetPassword(ResetPasswordRequest model);
    Task<IEnumerable<AccountResponse>> GetAll();
    Task<AccountResponse> GetById(int id);
    Task<AccountResponse> Create(CreateRequest model);
    Task<AccountResponse> Update(int id, UpdateRequest model);
    Task Delete(int id);
}

public class AccountService(
    IUnitOfWork unitOfWork,
    ITokenAuthService tokenAuthService,
    IMapper mapper,
    IOptions<AppSettings> appSettings,
    IEmailService emailService)
    : IAccountService
{
    private readonly AppSettings _appSettings = appSettings.Value;

    public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model, string ipAddress)
    {
        var account = await unitOfWork.AccountRepository.GetByEmail(model.Email);

        // validate
        if (account == null || !account.IsVerified || !BCrypt.Verify(model.Password, account.PasswordHash))
            throw new AppException("Email or password is incorrect");

        // authentication successful so generate jwt and refresh tokens
        var jwtToken = tokenAuthService.GenerateJwtToken(account);
        var refreshToken = await tokenAuthService.GenerateRefreshToken(ipAddress);
        await unitOfWork.AccountRepository.AddNewRefreshToken(account.Id, refreshToken);
        
        // remove old refresh tokens from account
        await  unitOfWork.AccountRepository.RemoveRefreshTokensOlderThanTtl(account.Id, _appSettings.RefreshTokenTTL);
        
        var response = mapper.Map<AuthenticateResponse>(account);
        response.JwtToken = jwtToken;
        response.RefreshToken = refreshToken.Token;
        return response;
    }

    public async Task<AuthenticateResponse> RefreshToken(string token, string ipAddress)
    {
        var account = await GetAccountByRefreshToken(token);
        var refreshToken = account.RefreshTokens.Single(x => x.Token == token);

        if (refreshToken.IsRevoked)
        {
            await  unitOfWork.AccountRepository.RevokeToken(account.Id, refreshToken.Token, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
        }

        if (!refreshToken.IsActive)
            throw new AppException("Invalid token");

        // replace old refresh token with a new one (rotate token)
        var newRefreshToken = await tokenAuthService.GenerateRefreshToken(ipAddress);
        await  unitOfWork.AccountRepository.RevokeToken(account.Id, token, ipAddress, "Replaced by new token");
        await  unitOfWork.AccountRepository.AddNewRefreshToken(account.Id, newRefreshToken);

        // remove old refresh tokens from account
        await  unitOfWork.AccountRepository.RemoveRefreshTokensOlderThanTtl(account.Id, _appSettings.RefreshTokenTTL);
        
        // generate new jwt
        var jwtToken = tokenAuthService.GenerateJwtToken(account);

        // return data in authenticate response object
        var response = mapper.Map<AuthenticateResponse>(account);
        response.JwtToken = jwtToken;
        response.RefreshToken = newRefreshToken.Token;
        return response;
    }

    public async Task RevokeToken(string token, string ipAddress)
    {
        var account = await GetAccountByRefreshToken(token);
        var refreshToken = account.RefreshTokens.Single(x => x.Token == token);

        if (!refreshToken.IsActive)
            throw new AppException("Invalid token");

        // revoke token and save
        await  unitOfWork.AccountRepository.RevokeToken(account.Id, token, ipAddress, "Revoked without replacement");
    }

    public async Task Register(RegisterRequest model, string origin)
    {
        // validate
        var existingAccount = await  unitOfWork.AccountRepository.GetByEmail(model.Email);
        if(existingAccount != null)
        {
            // send already registered error in email to prevent account enumeration
            SendAlreadyRegisteredEmail(model.Email, origin);
            return;
        }

        // map model to new account object
        var account = mapper.Map<Account>(model);

        // first registered account is an admin - TODO: THIS SHOULD LATER BE REPLACED...
        var isFirstAccount = !await  unitOfWork.AccountRepository.HasAny();
        account.Role = isFirstAccount ? Role.Admin : Role.User;
        account.CreatedDateTime = DateTime.UtcNow;
        account.VerificationToken = await GenerateVerificationToken();

        // hash password
        account.PasswordHash = BCrypt.HashPassword(model.Password);

        // save account
        await  unitOfWork.AccountRepository.Insert(account);

        // send email
        SendVerificationEmail(account, origin);
    }

    public async Task VerifyEmail(string token)
    {
        var account = await unitOfWork.AccountRepository.GetByVerificationToken(token);

        if (account == null) 
            throw new AppException("Verification failed");

        account.Verified = DateTime.UtcNow;
        account.VerificationToken = null;

        unitOfWork.AccountRepository.Update(account);
    }

    public async Task ForgotPassword(ForgotPasswordRequest model, string origin)
    {
        var account = await  unitOfWork.AccountRepository.GetByEmail(model.Email);

        // always return ok response to prevent email enumeration
        if (account == null) return;

        // create reset token that expires after 1 day
        account.ResetToken = await GenerateResetToken();
        account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

        unitOfWork.AccountRepository.Update(account);

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
        account.PasswordHash = BCrypt.HashPassword(model.Password);
        account.PasswordReset = DateTime.UtcNow;
        account.ResetToken = null;
        account.ResetTokenExpires = null;

        unitOfWork.AccountRepository.Update(account);
    }

    public Task<IEnumerable<AccountResponse>> GetAll()
    {
        throw new NotImplementedException();
    }

    public async Task<AccountResponse> GetById(int id)
    {
        var account = await GetAccountOrThrow(id);
        return mapper.Map<AccountResponse>(account);
    }

    public async Task<AccountResponse> Create(CreateRequest model)
    {
        // validate
        var existingAccount = await  unitOfWork.AccountRepository.GetByEmail(model.Email);
        if (existingAccount != null)
        {
            throw new AppException($"Email '{model.Email}' is already registered");
        }

        // map model to new account object
        var account = mapper.Map<Account>(model);
        account.CreatedDateTime = DateTime.UtcNow;
        account.Verified = DateTime.UtcNow;

        // hash password
        account.PasswordHash = BCrypt.HashPassword(model.Password);

        // save account
        unitOfWork.AccountRepository.Update(account);
        
        return mapper.Map<AccountResponse>(account);
    }

    public async Task<AccountResponse> Update(int id, UpdateRequest model)
    {
        var account = await GetAccountOrThrow(id);

        // validate
        if (account.Email != model.Email)
        {
            var existing = await  unitOfWork.AccountRepository.GetByEmail(model.Email);
            if (existing != null)
            {
                throw new AppException($"Email '{model.Email}' is already registered");
            }
        }
            

        // hash password if it was entered
        if (!string.IsNullOrEmpty(model.Password))
            account.PasswordHash = BCrypt.HashPassword(model.Password);

        // copy model to account and save
        mapper.Map(model, account);
        account.Updated = DateTime.UtcNow;

        unitOfWork.AccountRepository.Update(account);
        
        return mapper.Map<AccountResponse>(account);
    }

    public async Task Delete(int id)
    {
        await  unitOfWork.AccountRepository.Delete(id);
    }

    // helper methods

    private async Task<Account> GetAccountOrThrow(int id)
    {
        var account = await  unitOfWork.AccountRepository.GetById(id);
        if (account == null) throw new KeyNotFoundException("Account not found");
        return account;
    }

    private async Task<Account> GetAccountByRefreshToken(string token)
    {
        var account =  await  unitOfWork.AccountRepository.GetByRefreshToken(token);
        if (account == null) throw new AppException("Invalid token");
        return account;
    }

    private async Task<Account> GetAccountByValidResetToken(string token)
    {
        var account = await  unitOfWork.AccountRepository.GetByResetToken(token);
        if (account == null || account.ResetTokenExpires > DateTime.UtcNow)
        {
            throw new AppException("Invalid token");
        }

        return account;
    }

    private async Task<string> GenerateResetToken()
    {
        // token is a cryptographically strong random sequence of values
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        // ensure token is unique by checking against db
        var accountByResetToken = await  unitOfWork.AccountRepository.GetByResetToken(token);
        if (accountByResetToken != null)
            return await GenerateResetToken();
        
        return token;
    }

    private async Task<string> GenerateVerificationToken()
    {
        // token is a cryptographically strong random sequence of values
        var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

        // ensure token is unique by checking against db
        var accountByToken = await  unitOfWork.AccountRepository.GetByVerificationToken(token);
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
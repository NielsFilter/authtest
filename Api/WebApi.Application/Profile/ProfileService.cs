using AutoMapper;
using Microsoft.Extensions.Options;
using WebApi.Authorization;
using WebApi.Entities;
using WebApi.Helpers;
using WebApi.Models.Accounts;
using WebApi.Services;
using WebApi.Shared;

namespace WebApi.Data.Profile;

public interface IProfileService
{
    Task<AccountDto> GetById(int id);
    Task<AccountDto> Create(ProfileCreateRequest model);
    Task<AccountDto> Update(int id, ProfileUpdateRequest model);
    Task<AccountDto> UpdateWithRoles(int id, ProfileUpdateRequest model, List<Role> role);
    Task Delete(int id);
    Task<IEnumerable<AccountDto>> ListAllPaged(FilterPagedDto input, CancellationToken cancellationToken = default);
}

public class ProfileService(
    IRepositoryFactory repositoryFactory,
    IMapper mapper) : IProfileService
{
    private readonly IAccountRepository _accountRepository = repositoryFactory.CreateAccountRepository();

    public async Task<IEnumerable<AccountDto>> ListAllPaged(FilterPagedDto input, CancellationToken cancellationToken = default)
    {
        var accounts = await _accountRepository.SearchPaged(input, cancellationToken);
        return mapper.Map<IEnumerable<AccountDto>>(accounts);
    }

    public async Task<AccountDto> GetById(int id)
    {
        var account = await GetAccountOrThrow(id);
        return await MapAccountDto(account);
    }

    public async Task<AccountDto> Create(ProfileCreateRequest model)
    {
        // validate
        var existingAccount = await  _accountRepository.GetByEmail(model.Email);
        if (existingAccount != null)
        {
            throw new AppException($"Email '{model.Email}' is already registered");
        }

        // map model to new account object
        var account = mapper.Map<Account>(model);
        account.CreatedDateTime = DateTime.UtcNow;
        account.Verified = DateTime.UtcNow;

        // hash password
        account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        // save account
        await _accountRepository.Insert(account);
        
        return await MapAccountDto(account);
    }
    
    public async Task<AccountDto> UpdateWithRoles(int id, ProfileUpdateRequest model, List<Role> roles)
    {
        var account = await UpdateProfileInfo(id, model);
         
        if (roles.Count > 0)
        {
            // roles have been set, update them on the profile
            await _accountRepository.SetRoles(account.Id, roles);
        }

        return await MapAccountDto(account);
    }

    public async Task<AccountDto> Update(int id, ProfileUpdateRequest model)
    {
        var account = await UpdateProfileInfo(id, model);
        return await MapAccountDto(account);
    }

    private async Task<AccountDto> MapAccountDto(Account account)
    {
        //TODO: Return a different type for profile. Account is too tighly linked to authentication
        var roles = await _accountRepository.GetAccountRoles(account.Id);
        var accountDto = mapper.Map<AccountDto>(account);
        accountDto.Roles = roles.Select(x => x.ToString()).ToList();
        return accountDto;
    }

    private async Task<Account> UpdateProfileInfo(int id, ProfileUpdateRequest model)
    {
        var account = await GetAccountOrThrow(id);

        // validate
        if (account.Email != model.Email)
        {
            var existing = await  _accountRepository.GetByEmail(model.Email);
            if (existing != null)
            {
                throw new AppException($"Email '{model.Email}' is already registered");
            }
        }

        // hash password if it was entered
        if (!string.IsNullOrEmpty(model.Password))
            account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

        // copy model to account and save
        mapper.Map(model, account);
        account.Updated = DateTime.UtcNow;

        await _accountRepository.Update(account);
        return account;
    }

    public async Task Delete(int id)
    {
        await _accountRepository.Delete(id);
    }

    private async Task<Account> GetAccountOrThrow(int id)
    {
        var account = await  _accountRepository.GetById(id);
        if (account == null) throw new KeyNotFoundException("Account not found");
        return account;
    }
}
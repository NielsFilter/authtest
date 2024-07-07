using AutoMapper;
using WebApi.Accounts.Models;
using WebApi.Entities;

namespace WebApi.Helpers.Mapping;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<RefreshToken, RefreshTokenDto>();
        CreateMap<Account, AccountDto>();
        CreateMap<RegisterRequest, Account>();
        CreateMap<ProfileCreateRequest, Account>();
        CreateMap<ProfileUpdateRequest, Account>();
    }
}
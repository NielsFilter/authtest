using AutoMapper;
using WebApi.Entities;
using WebApi.Models.Accounts;

namespace WebApi.Helpers.Mapping;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<RefreshToken, RefreshTokenDto>();
        CreateMap<Account, AccountDto>();
        CreateMap<RegisterRequest, Account>();
        CreateMap<CreateRequest, Account>();
        CreateMap<UpdateRequest, Account>();
    }
}
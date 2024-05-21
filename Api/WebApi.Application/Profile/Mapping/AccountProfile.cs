using AutoMapper;
using WebApi.Entities;
using WebApi.Models.Accounts;

namespace WebApi.Helpers.Mapping;

public class AccountProfile : Profile
{
    public AccountProfile()
    {
        CreateMap<Account, AccountResponse>();
        CreateMap<Account, AuthenticateResponse>();
        CreateMap<RegisterRequest, Account>();
        CreateMap<CreateRequest, Account>();

        CreateMap<UpdateRequest, Account>()
            .ForAllMembers(x => x.Condition(
                (src, dest, prop) =>
                {
                    switch (prop)
                    {
                        // ignore null & empty string properties
                        case null:
                        case string propStr when string.IsNullOrEmpty(propStr):
                            return false;
                    }

                    // ignore null role
                    if (x.DestinationMember.Name == "Role" && src.Role == null)
                    {
                        return false;
                    }

                    return true;
                }
            ));
    }
}
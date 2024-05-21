using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WebApi.Data.Profile;
using WebApi.Entities;

namespace WebApi.Infrastructure;

public class CreateAuditInterceptor(ILoggedInUserResolver loggedInUserResolver) : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, 
        InterceptionResult<int> result)
    {
        if (eventData.Context is null) return result;

        var loggedInAccount = loggedInUserResolver.GetLoggedInAccount();
        if (loggedInAccount is null)
        {
            throw new InvalidOperationException("No logged in user found");
        }
        
        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            if (entry is not { State: EntityState.Added, Entity: ICreateAudit createAudit }) continue;
            createAudit.CreatedById = loggedInAccount.Id;
            createAudit.CreatedTimestamp = DateTime.UtcNow;
        }
        return result;
    }
}
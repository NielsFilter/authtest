using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Data.Profile;
using WebApi.Domain.Profile;
using WebApi.Infrastructure;

namespace WebApi.Helpers;

using Microsoft.EntityFrameworkCore;
using WebApi.Entities;

public class DataContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<AccountRole> AccountRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    private readonly ILoggedInUserResolver _loggedInUserResolver;
    private readonly IConfiguration Configuration;

    public DataContext(
        ILoggedInUserResolver loggedInUserResolver,
        IConfiguration configuration)
    {
        _loggedInUserResolver = loggedInUserResolver;
        Configuration = configuration;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to sqlite database
        options
            .UseSqlServer(Configuration.GetConnectionString("Default"),
                x => x.MigrationsAssembly("WebApi.Infrastructure"));
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entity in ChangeTracker.Entries())
        {
            var loggedInAccount = _loggedInUserResolver.GetLoggedInAccount();
            if (loggedInAccount is null)
            {
                break;
            }

            switch (entity)
            {
                case { Entity: ICreateAudit createAudit, State: EntityState.Added }:
                    createAudit.CreatedById = loggedInAccount.Id;
                    createAudit.CreatedTimestamp = DateTime.UtcNow;
                    break;
                case { Entity: IUpdateAudit updateAudit, State: EntityState.Modified }:
                    updateAudit.UpdatedById = loggedInAccount.Id;
                    updateAudit.UpdatedTimestamp = DateTime.UtcNow;
                    break;
                case { Entity: IDeleteAudit deleteAudit, State: EntityState.Deleted }:
                    entity.State = EntityState.Modified;
                    deleteAudit.IsDeleted = true;
                    deleteAudit.DeletedById = loggedInAccount.Id;
                    deleteAudit.DeletedTimestamp = DateTime.UtcNow;
                    break;
            }
        }
        
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.TargetAccount)
            .WithMany()
            .HasForeignKey(n => n.TargetAccountId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.CreatedBy)
            .WithMany()
            .HasForeignKey(n => n.CreatedById)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.UpdatedBy)
            .WithMany()
            .HasForeignKey(n => n.UpdatedById)
            .OnDelete(DeleteBehavior.NoAction);
        
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.DeletedBy)
            .WithMany()
            .HasForeignKey(n => n.DeletedById)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
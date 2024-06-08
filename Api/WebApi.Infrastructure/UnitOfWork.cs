using WebApi.Domain;
using WebApi.Helpers;

namespace WebApi.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _context;

    public UnitOfWork(DataContext context)
    {
        _context = context;
    }

    public async Task<T> Run<T>(Func<Task<T>> action, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.BeginTransactionAsync(cancellationToken);
            var result = await action();
            await _context.Database.CommitTransactionAsync(cancellationToken);
            return result;
        }
        catch (Exception)
        {
            await _context.Database.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task Run(Func<Task> action, CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.Database.BeginTransactionAsync(cancellationToken);
            await action();
            await _context.Database.CommitTransactionAsync(cancellationToken);
        }
        catch (Exception)
        {
            await _context.Database.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task Begin(CancellationToken cancellationToken = default)
    {
        await _context.Database.BeginTransactionAsync(cancellationToken);
    }
    
    public async Task Commit(CancellationToken cancellationToken = default)
    {
        await _context.Database.CommitTransactionAsync(cancellationToken);
    }
    
    public async Task Rollback(CancellationToken cancellationToken = default)
    {
        await _context.Database.RollbackTransactionAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using WebApi.Entities;
using WebApi.Services;

namespace WebApi.Helpers;

public class BaseRepository<T>(DataContext context) : IRepository<T>
    where T : Entity
{
    public async Task<bool> HasAny(CancellationToken cancellationToken = default)
    {
        return await context.Set<T>().AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<T>> ListAll(CancellationToken cancellationToken = default)
    {
        return await context.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<T?> GetById(int id, CancellationToken cancellationToken = default)
    {
        return await context.Set<T>().FindAsync(id, cancellationToken);
    }

    public async Task Insert(T entity, CancellationToken cancellationToken = default)
    {
        await context.Set<T>().AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task Update(T entity, CancellationToken cancellationToken = default)
    {
        context.Entry(entity).State = EntityState.Modified;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task Delete(int id, bool throwIfNotExists = false, CancellationToken cancellationToken = default)
    {
        var entity = await context.Set<T>().FindAsync(id);
        if (entity != null)
        {
            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
            return;
        }
        
        if(throwIfNotExists)
        {
            throw new AppException($"Entity {typeof(T).Name} with id {id} not found"); //todo: validation error?
        }
    }
    
    public void Dispose()
    {
        context.Dispose();
    }
}
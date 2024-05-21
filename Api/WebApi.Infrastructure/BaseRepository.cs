using Microsoft.EntityFrameworkCore;
using WebApi.Entities;
using WebApi.Services;

namespace WebApi.Helpers;

public class BaseRepository<T>(DataContext context) : IRepository<T>
    where T : Entity
{
    public async Task<bool> HasAny()
    {
        return await context.Set<T>().AnyAsync();
    }

    public async Task<IEnumerable<T>> ListAll()
    {
        return await context.Set<T>().AsNoTracking().ToListAsync();
    }

    public async Task<T?> GetById(int id)
    {
        return await context.Set<T>().FindAsync(id);
    }

    public async Task Insert(T entity)
    {
        await context.Set<T>().AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task Update(T entity)
    {
        context.Set<T>().Update(entity);
        await context.SaveChangesAsync();
    }

    public async Task Delete(int id, bool throwIfNotExists = false)
    {
        var entity = await context.Set<T>().FindAsync(id);
        if (entity != null)
        {
            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync();
            return;
        }
        
        if(throwIfNotExists)
        {
            throw new AppException($"Entity {typeof(T).Name} with id {id} not found"); //todo: validation error?
        }
    }
}
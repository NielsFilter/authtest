using WebApi.Entities;

namespace WebApi.Services;

public interface IRepository<T>
    where T : Entity
{
    Task<bool> HasAny();
     Task<IEnumerable<T>> ListAll();
     Task<T?> GetById(int id);
     Task Insert(T entity);
     Task Update(T entity);
     Task Delete(int id, bool throwIfNotExists = false);
}
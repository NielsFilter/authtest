using WebApi.Entities;

namespace WebApi.Services;

public interface IRepository<T> : IDisposable
    where T : Entity
{
    Task<bool> HasAny(CancellationToken cancellationToken = default);
     Task<IEnumerable<T>> ListAll(CancellationToken cancellationToken = default);
     Task<T?> GetById(int id, CancellationToken cancellationToken = default);
     Task Insert(T entity, CancellationToken cancellationToken = default);
     Task Update(T entity, CancellationToken cancellationToken = default);
     Task Delete(int id, bool throwIfNotExists = false, CancellationToken cancellationToken = default);
}
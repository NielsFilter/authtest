 using WebApi.Services;

 namespace WebApi.Domain;

 public interface IUnitOfWork : IDisposable
 {
     Task<T> Run<T>(Func<Task<T>> action, CancellationToken cancellationToken = default);
     Task Run(Func<Task> action, CancellationToken cancellationToken = default);
     Task Begin(CancellationToken cancellationToken = default);
     Task Commit(CancellationToken cancellationToken = default);
     Task Rollback(CancellationToken cancellationToken = default);
 }
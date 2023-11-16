using NHibernate;
using NHibernate.Criterion;
using System.Linq.Expressions;

namespace Infraestructure.Data;

public interface IRepository<T>
{
    ISession Session { get; }
    void Create(T entity);
    void Update(T entity);
    void Delete(T entity);
    void Flush();
    T? Get(int id);
    T? Get(Expression<Func<T, bool>> predicate);
    IEnumerable<T> List(Pagging? pagging = null);
    IEnumerable<T> List(Expression<Func<T, bool>> predicate, Pagging? pagging = null);
    IEnumerable<T> List(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order);
    IEnumerable<T> List(ICriterion criterion, Pagging? pagging = null);

    // Async
    Task<T> GetAsync(int id);
    Task<T> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task CreateAsync(T entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    Task FlushAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> predicate, Pagging? pagging = null, CancellationToken cancellationToken = default(CancellationToken));

    void Evict(T entity);
}
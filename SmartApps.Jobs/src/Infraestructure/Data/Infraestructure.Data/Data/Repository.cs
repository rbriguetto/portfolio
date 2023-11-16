using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using System.Linq.Expressions;

namespace Infraestructure.Data;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ITransactionManager _transactionManager;

    public Repository(ITransactionManager transactionManager)
    {
        _transactionManager = transactionManager;
    }

    public virtual IQueryable<T> Table =>
        Session.Query<T>();

    public ISession Session =>  _transactionManager.GetSession() 
        ?? throw new NullReferenceException("TransactionManager.GetSession() is null");

    public void Create(T entity)
    {
        Session.SaveOrUpdate(entity);
    }

    public T Get(int id)
    {
        return Session.Get<T>(id);
    }

    public T? Get(Expression<Func<T, bool>> predicate)
    {
        return Table.Where(predicate).FirstOrDefault();
    }

    public void Update(T entity)
    {
        Session.SaveOrUpdate(entity);
    }

    public void Delete(T entity)
    {
        Session.Delete(entity);
    }

    public void Flush()
    {
        Session.Flush();
    }

    public Task<T> GetAsync(int id)
    {
        return Session.GetAsync<T>(id);
    }

    public Task<T> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Table.Where(predicate).FirstOrDefaultAsync(cancellationToken);
    }

    public Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        return Session.SaveOrUpdateAsync(entity, cancellationToken);
    }

    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        return Session.SaveOrUpdateAsync(entity, cancellationToken);
    }

    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        return Session.DeleteAsync(entity, cancellationToken);
    }

    public Task FlushAsync(CancellationToken cancellationToken = default)
    {
        return Session.FlushAsync(cancellationToken);
    }

    public void Evict(T entity)
    {
        Session.Evict(entity);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pagging"></param>
    /// <returns></returns>
    public IEnumerable<T> List(Pagging? pagging = null)
    {
        var query = Session.QueryOver<T>();
        ApplyPagging(query, pagging);
        ApplyPaggingOrder(query, pagging);
        ApplyRowCount(query, pagging);
        return query.List().ToReadOnlyCollection();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="pagging"></param>
    /// <returns></returns>
    public IEnumerable<T> List(Expression<Func<T, bool>> predicate, Pagging? pagging = null)
    {
        var query = Session.QueryOver<T>();
        query.Where(predicate);
        ApplyPagging(query, pagging);
        ApplyPaggingOrder(query, pagging);
        ApplyRowCount(query, pagging, predicate);
        return query.List().ToReadOnlyCollection();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="criterion"></param>
    /// <param name="pagging"></param>
    /// <returns></returns>
    public IEnumerable<T> List(ICriterion criterion, Pagging? pagging = null)
    {
        var query = Session.CreateCriteria<T>()
            .Add(criterion);

        var rowCountQuery = Session.CreateCriteria<T>()
            .Add(criterion)
            .SetProjection(Projections.RowCount()).FutureValue<Int32>();

        if (pagging != null)
        {
            if (!string.IsNullOrEmpty(pagging.SortColumn))
            {
                query.AddOrder(new Order(pagging.SortColumn, pagging.SortDirection == "asc"));
            }

            query.SetFirstResult(pagging.GetStartIndex());
            query.SetMaxResults(pagging.PageSize);
        }

        var results = query.Future<T>();

        if (pagging != null && !pagging.DisableCount)
        {
            pagging.Total = rowCountQuery.Value;
        }

        return results;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="order"></param>
    /// <param name="pagging"></param>
    /// <returns></returns>
    public IEnumerable<T> List(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order)
    {
        return Fetch(predicate, order).ToReadOnlyCollection();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="query"></param>
    /// <param name="pagging"></param>
    public void ApplyPagging(IQueryOver<T> query, Pagging? pagging)
    {
        if (pagging == null) return;
        query.Take(pagging.PageSize).Skip((pagging.Page - 1) * pagging.PageSize);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="query"></param>
    /// <param name="pagging"></param>
    public void ApplyPaggingOrder(IQueryOver<T, T> query, Pagging? pagging)
    {
        if (pagging == null) return;
        if (string.IsNullOrEmpty(pagging.SortColumn)) return;
        switch (pagging.SortDirection.ToLower())
        {
            case "desc" :
                query.OrderBy(Projections.Property(pagging.SortColumn)).Desc();
                break;
            default : 
                query.OrderBy(Projections.Property(pagging.SortColumn)).Asc();
                break;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="query"></param>
    /// <param name="pagging"></param>
    public void ApplyRowCount(IQueryOver<T, T> query, Pagging? pagging, Expression<Func<T, bool>>? predicate = null)
    {
        if (pagging == null) return;
        var rowCountQuery = Session.QueryOver<T>();
        if (predicate != null) rowCountQuery.Where(predicate);
        pagging.Total = rowCountQuery
            .ToRowCountQuery()
            .FutureValue<int>().Value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public virtual IQueryable<T> Fetch(Expression<Func<T, bool>> predicate)
    {
        return Table.Where(predicate);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="order"></param>
    /// <returns></returns>
    public virtual IQueryable<T> Fetch(Expression<Func<T, bool>> predicate, Action<Orderable<T>> order)
    {
        var orderable = new Orderable<T>(Fetch(predicate));
        order(orderable);
        return orderable.Queryable;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="pagging"></param>
    /// <returns></returns>
    public async Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> predicate, Pagging? pagging = null, CancellationToken cancellationToken = default(CancellationToken))
    {
        var query = Session.QueryOver<T>();
        query.Where(predicate);
        ApplyPagging(query, pagging);
        ApplyPaggingOrder(query, pagging);
        ApplyRowCount(query, pagging, predicate);
        return (await query.ListAsync(cancellationToken)).ToReadOnlyCollection();
    }
}
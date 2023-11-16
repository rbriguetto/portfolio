using System.Data;
using NHibernate;

namespace Infraestructure.Data;

public class TransactionManager : ITransactionManager, IDisposable
{
    private readonly ISessionFactoryHolder _sessionFactoryHolder;
    private readonly IEnumerable<IInterceptor> _interceptors; 
    private ISession? _session = null;

    public TransactionManager(ISessionFactoryHolder sessionFactoryHolder,
        IEnumerable<IInterceptor> interceptors)
    {
        _interceptors = interceptors;
        _sessionFactoryHolder = sessionFactoryHolder;
        IsolationLevel = IsolationLevel.ReadCommitted;
    }

    public IsolationLevel IsolationLevel { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public void Cancel()
    {
        if (_session != null)
        {

            // IsActive is true if the transaction hasn't been committed or rolled back
            if (_session.GetTransaction() != null && _session.GetTransaction().IsActive)
            {
                _session.GetTransaction().Rollback();
            }

            DisposeSession();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Demand()
    {
        EnsureSession(IsolationLevel);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public ISession GetSession()
    {
        Demand();
        return _session ?? throw new NullReferenceException();
    }

    /// <summary>
    /// 
    /// </summary>
    public void RequireNew()
    {
        RequireNew(IsolationLevel);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="level"></param>
    public void RequireNew(IsolationLevel level)
    {
        DisposeSession();
        EnsureSession(level);
    }

    /// <summary>
    /// 
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        DisposeSession();
    }

    private void DisposeSession()
    {
        if (_session != null)
        {

            try
            {
                // IsActive is true if the transaction hasn't been committed or rolled back
                if (_session.GetCurrentTransaction() != null && _session.GetCurrentTransaction().IsActive)
                {
                    _session.GetCurrentTransaction().Commit();
                }
            }
            finally
            {
                _session.Close();
                _session.Dispose();
                _session = null;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="level"></param>
    private void EnsureSession(IsolationLevel level)
    {
        if (_session != null)
        {
            return;
        }

        var sessionFactory = _sessionFactoryHolder.GetSessionFactory();

        if (_interceptors.Any())
            _session = sessionFactory
                .WithOptions().Interceptor(_interceptors.FirstOrDefault())
                .FlushMode(FlushMode.Commit)
                .OpenSession();
        else
            _session = sessionFactory
                .WithOptions()
                .FlushMode(FlushMode.Commit)
                .OpenSession();

        _session.BeginTransaction(level);
    }
}
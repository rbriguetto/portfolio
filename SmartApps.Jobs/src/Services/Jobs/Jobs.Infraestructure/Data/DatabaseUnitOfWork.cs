using Infraestructure.Data;
using SmartApps.Jobs.Application;

namespace SmartApps.Jobs.Infraestructure.Data;

public class DatabaseUnitOfWork : IUnitOfWork
{
    private readonly ITransactionManager _transactionManager;

    public DatabaseUnitOfWork(ITransactionManager transactionManager)
    {
        _transactionManager = transactionManager;
    }

    public void BeginTransaction()
    {
        _transactionManager.RequireNew();
    }

    public void Commit()
    {
        _transactionManager.RequireNew();
    }

    public void Rollback()
    {
        _transactionManager.Cancel();
        _transactionManager.RequireNew();
    }
}
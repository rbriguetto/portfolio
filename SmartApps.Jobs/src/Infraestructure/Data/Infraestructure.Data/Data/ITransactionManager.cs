using NHibernate;
using System.Data;

namespace Infraestructure.Data;

public interface ITransactionManager
{
    void Demand();
    void RequireNew();
    void RequireNew(IsolationLevel level);
    void Cancel();

    ISession GetSession();
}
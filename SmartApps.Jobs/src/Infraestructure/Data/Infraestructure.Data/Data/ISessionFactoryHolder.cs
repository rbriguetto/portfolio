using NHibernate;

namespace Infraestructure.Data;

public interface ISessionFactoryHolder
{
    ISessionFactory GetSessionFactory();
}
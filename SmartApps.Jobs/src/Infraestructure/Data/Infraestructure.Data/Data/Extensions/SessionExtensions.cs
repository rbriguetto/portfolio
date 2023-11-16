using NHibernate;

namespace Infraestructure.Data;

public static class CustomSessionExtensions
{
    public static ITransaction? Implementation { get; set; } = null;

    /// <summary>
    /// Workaraound for mock
    /// </summary>
    /// <param name="session"></param>
    /// <returns></returns>
    public static ITransaction GetTransaction(this ISession session)
    {
        if (Implementation != null)
        {
            return Implementation;
        }

        return session.GetCurrentTransaction();
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using NHibernate;

namespace Infraestructure.Data;

public static class ServiceCollectionExtension 
{
    public static IServiceCollection AddDataInfraestructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<ITransactionManager, TransactionManager>();
        services.AddSingleton<ISessionFactoryHolder, SessionFactoryHolder>();
        return services;
    }
}
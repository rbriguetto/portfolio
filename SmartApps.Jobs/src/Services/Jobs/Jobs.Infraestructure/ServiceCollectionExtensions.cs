using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Infraestructure.Data;
using Infraestructure.Migrations;
using Infraestructure.RabbitMq;
using Infraestructure.RabbitMq.Models;
using SmartApps.Jobs.Application;
using SmartApps.Jobs.Infraestructure.Data;
using SmartApps.Jobs.Infraestructure.Elasticsearch;
using SmartApps.Jobs.Infraestructure.Workers;
using SmartApps.Jobs.Repositories;

namespace SmartApps.Jobs.Infraestructure;

public static class ServiceCollectionExtension 
{
    public static IServiceCollection AddSmartAppsJobsInfraestructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()) );

        services.Configure<ConnectionStringOptions>(configuration.GetSection(ConnectionStringOptions.Section));
        services.Configure<RabbitMqOptions>(configuration.GetSection(RabbitMqOptions.Section));

        services.AddDataInfraestructure(configuration);
        services.AddMigrationsInfraestructure();
        services.AddRabbitMqInfraestructure(configuration);

        services.AddScoped<IJobRepository, DatabaseJobRepository>();
        services.AddScoped<IJobEventRepository, ElasticsearchEventRepository>();
        services.AddScoped<IUnitOfWork, DatabaseUnitOfWork>();
        services.AddScoped<ElasticserviceClientProvider>();
        services.AddHostedService<MarkJobAsStartedWorker>();
        services.AddHostedService<MarkJobAsFinishedWorker>();
        return services;
    }
}
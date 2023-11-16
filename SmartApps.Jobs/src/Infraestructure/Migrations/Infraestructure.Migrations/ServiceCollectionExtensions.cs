using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using FluentMigrator.Runner.VersionTableInfo;
using FluentMigrator.Runner;
using Infraestructure.Data;

namespace Infraestructure.Migrations;

public static class ServiceCollectionExtension 
{
    public static IServiceCollection AddMigrationsInfraestructure(this IServiceCollection services)
    {
        services
            .AddFluentMigratorCore()
            .ConfigureRunner(options =>
            {
                var connectionStringConfig = options.Services
                    .BuildServiceProvider().GetRequiredService<IOptions<ConnectionStringOptions>>();
                options.Services.AddSingleton<IVersionTableMetaData, VersionTable>();
                var driverName = connectionStringConfig.Value.DriverName;
                var connectionString = connectionStringConfig.Value.ConnectionString;
                switch (driverName?.Trim().ToLower())
                {
                    case "sqlserver":
                        options.AddSqlServer();
                        break;
                    case "sqlite":
                        options.AddSQLite();
                        break;
                    case "mysql":
                        options.AddMySql5();
                        break;
                }
                options.WithGlobalConnectionString(connectionString);
                options.ScanIn(AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic).ToArray()).For.Migrations();
            })
            .AddLogging(lb => lb.AddFluentMigratorConsole());

        services.AddTransient<IStartupFilter, MigrationStartupFilter>();

        return services;
    }
}
using FluentMigrator.Runner;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infraestructure.Migrations;

public class MigrationStartupFilter : IStartupFilter
{
    protected readonly IServiceProvider _serviceProvider;
    protected readonly ILogger<MigrationStartupFilter> _logger;


    public MigrationStartupFilter(IServiceProvider serviceProvider,
        ILogger<MigrationStartupFilter> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return builder =>
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var runner = scope.ServiceProvider.GetService<IMigrationRunner>();
                    if (runner == null)
                    {
                        _logger.LogError("IMigrationRunner not found!");
                        return;
                    }

                    runner.MigrateUp();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("The following error ocurred: {error_msg} {stack_trace}", ex.Message, ex.StackTrace);
            }

            next(builder);
        };
    }
}
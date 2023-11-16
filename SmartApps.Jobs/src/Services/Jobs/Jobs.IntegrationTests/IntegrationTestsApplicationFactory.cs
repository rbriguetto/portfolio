using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using Xunit;
using Infraestructure.Data;
using Infraestructure.Migrations;
using Infraestructure.RabbitMq.Models;

namespace SmartApps.Jobs.IntegrationTests;

public class IntegrationTestsApplicationFactory: WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _mssqlContainer = new MsSqlBuilder().Build();
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder().Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.Configure<ConnectionStringOptions>(options => { 
                options.ConnectionString = _mssqlContainer.GetConnectionString();
                options.DriverName =  "sqlserver";
                options.TablePrefix = "tests_";
            });
            
            services.Configure<RabbitMqOptions>(options => { 
                options.Uri = _rabbitMqContainer.GetConnectionString();
            });

            services.AddMigrationsInfraestructure();
        });
        builder.UseEnvironment("IntegrationTests");
    }

    public async Task InitializeAsync()
    {
        await _mssqlContainer.StartAsync();
        await _rabbitMqContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _mssqlContainer.StopAsync();
        await _rabbitMqContainer.StopAsync();
    }
}

using System.Data.SqlClient;
using Testcontainers.MsSql;

namespace Infraestructure.Data.IntegrationTests;

public class IntegrationTestsFixture : IDisposable
{
    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder().Build();
    public readonly IServiceCollection _serviceCollection = new ServiceCollection();
    public readonly IServiceProvider Services;
    public readonly IConfiguration Configuration;
    

    public IntegrationTestsFixture()
    {
        _dbContainer.StartAsync().GetAwaiter().GetResult();
        CreateInitialData(_dbContainer.GetConnectionString()).GetAwaiter().GetResult();

        Configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        _serviceCollection.AddLogging(configure => configure.SetMinimumLevel(LogLevel.Debug));
        _serviceCollection.AddSingleton(Configuration);
        _serviceCollection.AddDataInfraestructure(Configuration);
        _serviceCollection.Configure<ConnectionStringOptions>(options => {
            options.ConnectionString = _dbContainer.GetConnectionString();
            options.DriverName = "sqlserver";
        });
        Services = _serviceCollection.BuildServiceProvider();
    }

    protected async Task CreateInitialData(string connectionString)
    {
        const string initFilename = "init_database.sql";
        if (!File.Exists(initFilename))
        {
            return;
        }

        var script = await File.ReadAllTextAsync(initFilename, CancellationToken.None);

        if (string.IsNullOrEmpty(script))
        {
            return;
        }

        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        var command = new SqlCommand(script, connection);
        await command.ExecuteNonQueryAsync();
        await connection.CloseAsync();
    }

    public void Dispose()
    {
        _dbContainer.StopAsync().GetAwaiter().GetResult();
        GC.SuppressFinalize(this);
    }
}

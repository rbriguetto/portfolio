using Microsoft.Extensions.Options;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.Extensions.Configuration;
using NHibernate;
using System.Reflection;

namespace Infraestructure.Data;

public class SessionFactoryHolder : ISessionFactoryHolder
{
    private ISessionFactory? _sessionFactory = null;
    private readonly ConnectionStringOptions _connectionStringConfig;

    public SessionFactoryHolder(IOptions<ConnectionStringOptions> connectionStringConfig)
    {
        _connectionStringConfig = connectionStringConfig.Value;
    }

    public ISessionFactory GetSessionFactory()
    {
        if (_sessionFactory == null)
        {
            var driverName = _connectionStringConfig.DriverName;
            var connectionString = _connectionStringConfig.ConnectionString;
            if (string.IsNullOrEmpty(driverName))
                throw new ArgumentException(nameof(ConnectionStringOptions.DriverName));
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException(nameof(ConnectionStringOptions.ConnectionString));

            var configure = Fluently
                .Configure();

            switch (driverName?.ToLower())
            {
                case "sqlserver":
                    configure = configure.Database(MsSqlConfiguration.MsSql2012.ConnectionString(connectionString));
                    break;
                case "sqlite":
                    configure = configure.Database(SQLiteConfiguration.Standard
                        .IsolationLevel(System.Data.IsolationLevel.ReadUncommitted)
                        .UsingFile(GetSQLiteDatabaseFileName(connectionString)));
                    break;
                case "mysql":
                    configure = configure.Database(MySQLConfiguration.Standard.ConnectionString(connectionString));
                    break;
            }

            configure = configure.Mappings(m =>
            {
                var recordAutoMapping = new RecordAutoMapping();
                var assembliesToMap = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => !string.IsNullOrEmpty(x.FullName) 
                        && !x.FullName.Contains(nameof(FluentNHibernate), StringComparison.InvariantCultureIgnoreCase))
                    .ToList() ?? Enumerable.Empty<Assembly>();

                foreach(var assemblyToMap in assembliesToMap)
                {
                    m.FluentMappings.AddFromAssembly(assemblyToMap);
                    m.AutoMappings.Add(AutoMap.Assembly(assemblyToMap, recordAutoMapping)
                        .Conventions.Add(new TableNameConvention(_connectionStringConfig.TablePrefix)));
                }
            });


            NHibernateLogger.SetLoggersFactory(new NHibernate.Logging.Serilog.SerilogLoggerFactory());
            
            var configuration = configure.BuildConfiguration();

           _sessionFactory = configuration .BuildSessionFactory();
        }

        return _sessionFactory;
    }

    public static string GetSQLiteDatabaseFileName(string connectionString) {
        
        var filename = connectionString.Remove(0, connectionString.IndexOf("Data Source=") + "Data Source=".Length);
        if (filename.IndexOf(";") != -1) {
            filename = filename.Substring(0, filename.IndexOf(";"));
        }

        return filename;
    }

}
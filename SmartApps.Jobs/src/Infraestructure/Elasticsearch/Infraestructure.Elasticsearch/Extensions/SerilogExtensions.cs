using Elastic.CommonSchema.Serilog;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Filters;
using Serilog.Sinks.Elasticsearch;

using Infraestructure.Elasticsearch.Middlewares;

namespace Infraestructure.Elasticsearch.Extensions;

public static class SerilogExtension
{
    public static WebApplicationBuilder AddSerilog(this WebApplicationBuilder builder, IConfiguration configuration, string applicationName)
    {
        var loggerFactory = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("ApplicationName", $"{applicationName} - {builder.Environment.EnvironmentName}")
            .Enrich.WithCorrelationId()
            .Enrich.WithExceptionDetails()
            .Filter.ByExcluding(Matching.FromSource("Microsoft.AspNetCore.StaticFiles"))
            .WriteTo.Async(writeTo => writeTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"));

        var options = ElasticsearchOptions.FromConfiguration(configuration);
        if (!string.IsNullOrEmpty(options.Host))
        {
            loggerFactory.WriteTo.Async(writeTo => writeTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(options.Host))
            {
                TypeName = null,
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                IndexFormat =  options.IndexName,
                BatchAction = ElasticOpType.Create,
                CustomFormatter = new EcsTextFormatter(),
                InlineFields = true,
                ModifyConnectionSettings = x => x.BasicAuthentication(options.Username, options.Password)
                    .ServerCertificateValidationCallback((o, certificate, arg3, arg4) => true )
            }));
        }
        
        Log.Logger = loggerFactory.CreateLogger();

        builder.Logging.ClearProviders();
        builder.Host.UseSerilog(Log.Logger, true);

        return builder;
    }

    public static WebApplication UseSerilog(this WebApplication app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        app.UseSerilogRequestLogging(opts =>
        {
            opts.EnrichDiagnosticContext = LogEnricherExtensions.EnrichFromRequest;
        });
        return app;
    }
}

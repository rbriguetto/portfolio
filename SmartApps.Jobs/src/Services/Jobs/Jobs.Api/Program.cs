using System.Globalization;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Localization;
using Serilog;
using Infraestructure.Elasticsearch.Extensions;
using SmartApps.Jobs.Infraestructure;

try {
 
    var builder = WebApplication.CreateBuilder(args);
    builder.AddSerilog(builder.Configuration, "SmartApps.Jobs.Api");
    builder.Configuration
        .AddJsonFile("appsettings.json", true)
        .AddJsonFile($"appsettings{builder.Environment.EnvironmentName}.json", true)
        .AddEnvironmentVariables();

    builder.Services.Configure<ElasticsearchOptions>(builder.Configuration.GetSection(ElasticsearchOptions.Section));
    builder.Services.AddControllers().AddJsonOptions(options => { 
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); 
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddSmartAppsJobsInfraestructure(builder.Configuration);

    var app = builder.Build();

    app.UseSerilog();

    var supportedCultures = new[]
    {
     new CultureInfo("pt-BR"),
    };

    app.UseRequestLocalization(new RequestLocalizationOptions
    {
        DefaultRequestCulture = new RequestCulture("pt-BR"),
        SupportedCultures = supportedCultures,
        SupportedUICultures = supportedCultures
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseExceptionHandler("/Error");
    }
    app.UseRouting();
    app.MapControllers();

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}

public partial class Program { 
    protected Program()
    {

    }
}
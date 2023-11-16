using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infraestructure.RabbitMq.Services;

namespace Infraestructure.RabbitMq;

public static class ServiceCollectionExtension 
{
    public static IServiceCollection AddRabbitMqInfraestructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IRabbitMqClient, DefaultRabbitMqClient>();
        return services;
    }
}
using Microsoft.Extensions.Configuration;

namespace Infraestructure.RabbitMq.Models;

public class RabbitMqOptions
{
    public static readonly string Section = "RabbitMq";

    public string Uri { get; set; } = string.Empty;

    public static RabbitMqOptions FromConfiguration(IConfiguration configuration) => new RabbitMqOptions()
        {
            Uri = configuration[$"{Section}:{nameof(Uri)}"] ?? string.Empty,
        };
}
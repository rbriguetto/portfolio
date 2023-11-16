using System.Text;
using MediatR;
using Microsoft.Extensions.Logging;
using Infraestructure.RabbitMq.Services;
using Infraestructure.Utils.Extensions;
using SmartApps.Jobs.Application.Notifications;

namespace Jobs.Infraestructure.DomainEvents;

public class PublishJobToQueueDomainEvent : INotificationHandler<JobCreatedNotification>
{
    private readonly IRabbitMqClient _rabbitMqClient;
    private readonly ILogger<PublishJobToQueueDomainEvent> _logger;

    private static readonly string queue = "SmartApps.Jobs.JobCreated";
    private static readonly string exchange = "Jobs.JobCreated";

    public PublishJobToQueueDomainEvent(
        IRabbitMqClient rabbitMqClient, 
        ILogger<PublishJobToQueueDomainEvent> logger)
    {
        _rabbitMqClient = rabbitMqClient;
        _logger = logger;
    }

    public Task Handle(JobCreatedNotification notification, CancellationToken cancellationToken)
    {
        try
        {
            _rabbitMqClient.EnsureExchangesAndQueuesAreCreated(new[] { queue }, new[] { exchange }, 
                new[] { new KeyValuePair<string, string>(queue, exchange)});
            _rabbitMqClient.Channel?.BasicPublish(exchange, string.Empty, true, null, Encoding.UTF8.GetBytes(notification.ToJson()));
            _logger.LogInformation("Job {job.id} publicado na exchange {exchange} com sucesso!", notification.JobId, exchange);
        }
        catch (Exception ex)
        {
            _logger.LogError("Ocorreu um erro ao publicar a mensagem na exchange {exchange}. {error_msg} {stack_trace}", 
                exchange, ex.Message, ex.StackTrace);
        }
        finally
        {
            _rabbitMqClient?.Dispose();
        }

        return Task.CompletedTask;
    }
}

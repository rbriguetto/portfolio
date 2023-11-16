using System.Text;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Infraestructure.RabbitMq.Services;
using Infraestructure.Utils.Extensions;
using SmartApps.Jobs.Application.Commands;

namespace SmartApps.Jobs.Infraestructure.Workers;

public class MarkJobAsFinishedWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MarkJobAsFinishedWorker> _logger;

    public MarkJobAsFinishedWorker(
        IServiceProvider serviceProvider,
        ILogger<MarkJobAsFinishedWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        _logger.LogInformation("Service started.");
        var queue = "SmartApps.Jobs.MarkJobAsCompleted";
        var exchange = "Jobs.JobCompleted";
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope(); 
                var rabbitMqClient = scope.ServiceProvider.GetRequiredService<IRabbitMqClient>();
                rabbitMqClient.EnsureExchangesAndQueuesAreCreated(new[] { queue }, new[] { exchange }, new[] { new KeyValuePair<string, string>(queue, exchange)});
                var consumer = new EventingBasicConsumer(rabbitMqClient.Channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var payload = Encoding.UTF8.GetString(body);
                    await HandleMessage(payload, scope, stoppingToken);
                    rabbitMqClient.Channel?.BasicAck(ea.DeliveryTag, false);
                };

                rabbitMqClient.Channel?.BasicConsume(
                    queue: queue,
                    autoAck: false,
                    consumer: consumer
                );

                await Task.Delay(-1, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("The following error ocurred: {error_msg} {stack_strack}", ex.Message, ex.StackTrace);
            }
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
        _logger.LogInformation("Service finished.");
    }

    private async Task HandleMessage(string payload, IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var command = payload.FromJson<MarkJobAsFinishedCommand>();
            if (command == null) 
            {
                return;
            }
            await mediator.Send(command, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("The following error ocurred when processing the {payload}: {error_msg} {stack_trace}", payload, ex.Message, ex.StackTrace);
        }
    }
}
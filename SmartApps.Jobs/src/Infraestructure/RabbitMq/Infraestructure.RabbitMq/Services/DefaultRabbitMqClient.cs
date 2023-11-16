using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Infraestructure.RabbitMq.Models;

namespace Infraestructure.RabbitMq.Services;

public class DefaultRabbitMqClient : IRabbitMqClient
{
    private readonly object lockObject = new object();
    private readonly ILogger<DefaultRabbitMqClient > _logger;
    private IModel? _channel = null;
    private IConnection? _connection = null;

    private readonly RabbitMqOptions _options;

    public DefaultRabbitMqClient(
        IOptions<RabbitMqOptions> options, 
        ILogger<DefaultRabbitMqClient> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public IModel? Channel { 
        get 
        { 
            EnsureChannelIsCreated();
            return _channel;
        }
    }

    private void EnsureChannelIsCreated()
    {
        if (_channel != null)
            return;
        
        lock(lockObject)
        {
            if (_channel != null)
                return;

            var factory = new ConnectionFactory()
            {
                Uri = new Uri(_options.Uri)
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }
    }

    public void EnsureExchangesAndQueuesAreCreated(IEnumerable<string> queues, IEnumerable<string> exchanges, IEnumerable<KeyValuePair<string, string>> queueExchangeBindings)
    {
        foreach(var exchange in exchanges)
            Channel.ExchangeDeclare(exchange: exchange, type: "fanout", durable: true, autoDelete: false);

        foreach(var queue in queues)
        {
            var args = new Dictionary<string, object>();

            if (queue.Contains(":Delayed:"))
            {
                var nonDelayedQueueName = queue.Substring(0, queue.LastIndexOf(":") + 1).Replace(":Delayed:", "");
                var delayTime = Convert.ToInt32(queue.Substring(queue.LastIndexOf(":") + 1));
                Channel?.QueueDeclare(queue: nonDelayedQueueName, durable: true, autoDelete: false, exclusive: false);

 
                args.Add("x-message-ttl", delayTime);
                args.Add("x-dead-letter-exchange", "");
                args.Add("x-dead-letter-routing-key", nonDelayedQueueName);
            }
            Channel?.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false, args);
        }

        foreach(var binding in queueExchangeBindings)
            Channel.QueueBind(binding.Key, binding.Value, "");
    }

    ~DefaultRabbitMqClient()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        try
        {
            if (Channel != null && Channel.IsOpen)
                Channel.Close();

            if (_connection != null && _connection.IsOpen)
                _connection.Close();
        }
        catch (Exception ex)
        {
            _logger.LogError("The following error ocurred: {error_msg} {stack_trace}", ex.Message, ex.StackTrace);
        }
    }
}
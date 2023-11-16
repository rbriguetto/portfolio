using RabbitMQ.Client;

namespace Infraestructure.RabbitMq.Services;

public interface IRabbitMqClient : IDisposable
{
    void EnsureExchangesAndQueuesAreCreated(IEnumerable<string> queues, IEnumerable<string> exchanges, 
        IEnumerable<KeyValuePair<string, string>> queueExchangeBindings);
    IModel? Channel { get; }
}
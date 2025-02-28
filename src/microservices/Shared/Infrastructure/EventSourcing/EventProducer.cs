using System.Text.Json;
using Common.Events;
using Common.EventSourcing;
using Confluent.Kafka;
using DnsClient.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.EventSourcing;

public class EventProducer(IOptions<ProducerConfig> optionsConfig, ILogger<EventProducer> logger) : IEventProducer
{
    private readonly ProducerConfig _config = optionsConfig.Value;

    public async Task ProduceAsync<T>(string topic, T @event) where T : DomainEvent
    {
        using var producer = new ProducerBuilder<string, string>(_config)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(Serializers.Utf8)
            .Build();

        var eventMessage = new Message<string, string>{
            Key = Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(@event, @event.GetType())
        };

        var deliveryResults = await producer.ProduceAsync(topic, eventMessage);
        logger.LogInformation($"Message was delivered to the topic - {topic} with key - {eventMessage.Key}");
        
        if (deliveryResults.Status == PersistenceStatus.NotPersisted)
        {
            throw new Exception($"Could not use {@event.GetType().Name} message to topic - {topic} due to the following reason: {deliveryResults.Message}!");
        }
    }
}

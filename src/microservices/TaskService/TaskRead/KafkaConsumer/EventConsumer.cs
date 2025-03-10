using System.Text.Json;
using Common.Events;
using Confluent.Kafka;
using FastEndpoints;
using Microsoft.Extensions.Options;

namespace TaskRead.KafkaConsumer;

public class EventConsumer(
    IOptions<ConsumerConfig> config,
    IUniversalEventHandler eventHandler,
    ILogger<EventConsumer> logger
) : IEventConsumer
{
    private readonly ConsumerConfig _config = config.Value;

    public void Consume(string topic, CancellationToken ct)
    {
        logger.LogInformation("Creating consumer for topic {Topic}, {Host}", topic, _config.BootstrapServers);
        using IConsumer<string, string>? consumer = new ConsumerBuilder<string, string>(_config)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();

        logger.LogInformation("Consumer created for topic {Topic}", topic);
        
        consumer.Subscribe(topic);
        
        logger.LogInformation("Subscribed to topic {Topic}", topic);
        var options = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };

        while (true)
        {
            ConsumeResult<string, string>? consumerResult = consumer.Consume();
            if (consumerResult?.Message == null)
            {
                continue;
            }

            logger.LogInformation("Consumed message from topic {Topic} with key {Key}", topic, consumerResult.Message.Key);

            var @event = JsonSerializer.Deserialize<DomainEvent>(consumerResult.Message.Value, options);
            logger.LogDebug("Deserialized event {@Event}", @event);
            
            var handlerMethod = eventHandler.GetType()
                .GetMethod(nameof(IUniversalEventHandler.HandleAsync), [@event!.GetType(), ct.GetType()]);

            if (handlerMethod == null)
            {
                logger.LogError("Could not find event handler method for event type {EventType}", @event.GetType());
                throw new ArgumentNullException(nameof(handlerMethod), "Could not find event handler method!");
            }

            handlerMethod.Invoke(eventHandler, [@event, ct]);

            consumer.Commit(consumerResult);
            logger.LogInformation("Committed offset for message with key {Key}", consumerResult.Message.Key);
        }
    }
}
using System.Text.Json;
using Common.Events;
using Confluent.Kafka;
using FastEndpoints;
using Microsoft.Extensions.Options;

namespace TaskRead.KafkaConsumer;

public class EventConsumer(
    IOptions<ConsumerConfig> config,
    IEventHandler eventHandler,
    ILogger<EventConsumer> logger
) : IEventConsumer
{
    private readonly ConsumerConfig _config = config.Value;

    public void Consume(string topic)
    {
        using IConsumer<string, string>? consumer = new ConsumerBuilder<string, string>(_config)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();

        consumer.Subscribe(topic);
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

            var handlerMethod = eventHandler.GetType().GetMethod("On", new[] { @event!.GetType() });

            if (handlerMethod == null)
            {
                logger.LogError("Could not find event handler method for event type {EventType}", @event.GetType());
                throw new ArgumentNullException(nameof(handlerMethod), "Could not find event handler method!");
            }

            handlerMethod.Invoke(eventHandler, new[] { @event });

            consumer.Commit(consumerResult);
            logger.LogInformation("Committed offset for message with key {Key}", consumerResult.Message.Key);
        }
    }
}
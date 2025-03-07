using System.Text.Json;
using Common.Events;
using Confluent.Kafka;
using FastEndpoints;
using Microsoft.Extensions.Options;

namespace TaskRead.KafkaConsumer;

public class EventConsumer(
    IOptions<ConsumerConfig> config,
    IEventHandler eventHandler
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
        var options = new JsonSerializerOptions{Converters ={new EventJsonConverter()}};

        while (true)
        {
            ConsumeResult<string, string>? consumerResult = consumer.Consume();

            if (consumerResult?.Message == null)
            {
                continue;
            }

            // create a json options with our custom converter
            var @event = JsonSerializer.Deserialize<DomainEvent>(consumerResult.Message.Value, options);
            
            
            // todo run the event handler in a separate thread
            
            // IEventBus eventBus = new EventBus();
            
            var handlerMethod = eventHandler.GetType().GetMethod("On", [@event!.GetType()]);

            if (handlerMethod == null)
            {
                throw new ArgumentNullException(nameof(handlerMethod), "Could not find event handler method!");
            }

            handlerMethod.Invoke(eventHandler, [@event]);

            // commits an offset for the message. Hence, next time it will start from the next message
            consumer.Commit(consumerResult);
        }
    }
}

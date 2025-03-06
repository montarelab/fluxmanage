using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Events;
using Common.Events.Models;

namespace TaskRead.KafkaConsumer;

public class EventJsonConverter : JsonConverter<DomainEvent>
{
    public override bool CanConvert(Type typeToConvert)
    {
        // checks if an argument is assignable to a variable of type BaseEvent
        return typeToConvert.IsAssignableFrom(typeof(DomainEvent));
    }

    public override DomainEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // attempts to parse JsonDocument from the reader
        if (!JsonDocument.TryParseValue(ref reader, out var doc))
        {
            throw new JsonException($"Failed to parse {nameof(JsonDocument)}!");
        }

        // tries to get the Type property from the root element
        if (!doc.RootElement.TryGetProperty("Type", out var type))
            // Discriminator property acts as a tag to identify the type of the event
        {
            throw new JsonException("Could not detect the Type discriminator property!");
        }

        string? typeDiscriminator = type.GetString();
        string jsonString = doc.RootElement.GetRawText();

        return typeDiscriminator switch{
            nameof(TaskCreatedEvent) => JsonSerializer.Deserialize<TaskCreatedEvent>(jsonString, options),
            nameof(TaskUpdatedEvent) => JsonSerializer.Deserialize<TaskUpdatedEvent>(jsonString, options),
            nameof(TaskDeletedEvent) => JsonSerializer.Deserialize<TaskDeletedEvent>(jsonString, options),
            nameof(TaskCompletedEvent) => JsonSerializer.Deserialize<TaskCompletedEvent>(jsonString, options),
            nameof(TaskAssignedEvent) => JsonSerializer.Deserialize<TaskAssignedEvent>(jsonString, options),
            nameof(ProjectCreatedEvent) => JsonSerializer.Deserialize<ProjectCreatedEvent>(jsonString, options),
            nameof(ProjectUpdatedEvent) => JsonSerializer.Deserialize<ProjectUpdatedEvent>(jsonString, options),
            nameof(ProjectDeletedEvent) => JsonSerializer.Deserialize<ProjectDeletedEvent>(jsonString, options),
            nameof(EpicCreatedEvent) => JsonSerializer.Deserialize<EpicCreatedEvent>(jsonString, options),
            nameof(EpicUpdatedEvent) => JsonSerializer.Deserialize<EpicUpdatedEvent>(jsonString, options),
            nameof(EpicDeletedEvent) => JsonSerializer.Deserialize<EpicDeletedEvent>(jsonString, options),
            _ => throw new JsonException($"{typeDiscriminator} is not supported yet!")
        };
    }

    public override void Write(Utf8JsonWriter writer, DomainEvent value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}

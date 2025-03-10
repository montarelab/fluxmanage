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
        Console.WriteLine($"Type to convert: {typeToConvert} Can convert: {typeToConvert.IsAssignableFrom(typeof(DomainEvent))}");
        return typeToConvert.IsAssignableFrom(typeof(DomainEvent));
    }

    public override DomainEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Console.WriteLine("Start reading");
        // attempts to parse JsonDocument from the reader
        if (!JsonDocument.TryParseValue(ref reader, out var doc))
        {
            throw new JsonException($"Failed to parse {nameof(JsonDocument)}!");
        }

        // tries to get the Type property from the root element
        if (!doc.RootElement.TryGetProperty(nameof(EventModel.EventType), out var eventType))
            // Discriminator property acts as a tag to identify the type of the event
        {
            throw new JsonException("Could not detect the Type discriminator property!");
        }

        string? typeDiscriminator = eventType.GetString();
        string jsonString = doc.RootElement.GetRawText();

        Console.WriteLine($"Type discriminator: {typeDiscriminator}");
        
        return typeDiscriminator switch{
            nameof(TicketCreatedEvent) => JsonSerializer.Deserialize<TicketCreatedEvent>(jsonString, options),
            nameof(TicketUpdatedEvent) => JsonSerializer.Deserialize<TicketUpdatedEvent>(jsonString, options),
            nameof(TicketDeletedEvent) => JsonSerializer.Deserialize<TicketDeletedEvent>(jsonString, options),
            nameof(TicketCompletedEvent) => JsonSerializer.Deserialize<TicketCompletedEvent>(jsonString, options),
            nameof(TicketAssignedEvent) => JsonSerializer.Deserialize<TicketAssignedEvent>(jsonString, options),
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

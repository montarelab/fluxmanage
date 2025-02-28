using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Events;

public class EventModel
{
    [BsonId]// BSON is Binary JSON
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public DateTime TimeStamp { get; set; }
    
    public Guid AggregateIdentifier { get; set; }// an Id of an aggregate that the event relates to

    public string AggregateType { get; set; } = string.Empty;

    public int Version { get; set; }

    public string EventType { get; set; } = string.Empty;

    public DomainEvent? EventData { get; set; }
}

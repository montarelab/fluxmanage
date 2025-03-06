namespace TaskRead.KafkaConsumer;

public interface IEventConsumer
{
    void Consume(string topic);
}

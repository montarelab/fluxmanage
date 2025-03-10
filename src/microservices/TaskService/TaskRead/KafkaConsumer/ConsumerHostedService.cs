namespace TaskRead.KafkaConsumer;

public class ConsumerHostedService(
    ILogger<ConsumerHostedService> logger,
    IServiceProvider serviceProvider
) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Event Consumer service starting.");

        using (var scope = serviceProvider.CreateScope())
        {
            var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
            string? topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
            logger.LogInformation("KAFKA_TOPIC: {Topic}", topic);
            if (string.IsNullOrEmpty(topic))
            {
                logger.LogError("KAFKA_TOPIC environment variable is not set.");
                throw new InvalidOperationException("KAFKA_TOPIC environment variable is not set.");
            }

            Task.Run(() => eventConsumer.Consume(topic, cancellationToken), cancellationToken);
        }

        logger.LogInformation("Event Consumer service started.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Event Consumer service stopping.");
        return Task.CompletedTask;
    }
}
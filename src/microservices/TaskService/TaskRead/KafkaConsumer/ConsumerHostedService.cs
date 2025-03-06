namespace TaskRead.KafkaConsumer;

public class ConsumerHostedService(
    ILogger<ConsumerHostedService> logger,
    IServiceProvider serviceProvider
) : IHostedService
{

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Event Consumer service running.");

        using (var scope = serviceProvider.CreateScope())
        {
            var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
            string? topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");

            Task.Run(action: () => eventConsumer.Consume(topic!), cancellationToken);
        }

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Event Consumer service stopped.");
        return Task.CompletedTask;
    }
}

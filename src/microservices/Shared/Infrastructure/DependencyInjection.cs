using Common.Auth;
using Common.EventSourcing;
using Infrastructure.EventSourcing;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddEventSourcingInfrastructure(this IServiceCollection services)
    {
        return services
            .AddScoped<ICurrentUserService, CurrentUserService>()
            .AddScoped<IEventStoreRepository, MongoDbEventStoreRepository>()
            .AddScoped<IEventProducer, EventProducer>()
            .AddScoped<IEventStore, EventStore>()
            .AddScoped(typeof(IEventSourcingHandler<>), typeof(EventSourcingHandler<>));
    } 
}
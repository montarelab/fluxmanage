using Common.Auth;
using Common.EventSourcing;
using Infrastructure.EventSourcing;
using Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;
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
    
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        return services
            .AddEventSourcingInfrastructure()
            .AddScoped<ExceptionMiddleware>();
    }
    
    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}
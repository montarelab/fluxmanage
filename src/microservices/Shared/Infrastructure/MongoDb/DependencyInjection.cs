using Common.Config;
using Common.Events.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;

namespace Infrastructure.MongoDb;

public static class DependencyInjection
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var conventionPack = new ConventionPack
        {
            new IgnoreIfNullConvention(true)
        };
        ConventionRegistry.Register("IgnoreNulls", conventionPack, _ => true);
        
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        BsonClassMap.RegisterClassMap<ProjectCreatedEvent>();
        BsonClassMap.RegisterClassMap<ProjectUpdatedEvent>();
        BsonClassMap.RegisterClassMap<ProjectDeletedEvent>();

        BsonClassMap.RegisterClassMap<TicketCreatedEvent>();
        BsonClassMap.RegisterClassMap<TicketDeletedEvent>();
        BsonClassMap.RegisterClassMap<TicketUpdatedEvent>();

        BsonClassMap.RegisterClassMap<EpicCreatedEvent>();
        BsonClassMap.RegisterClassMap<EpicUpdatedEvent>();
        BsonClassMap.RegisterClassMap<EpicDeletedEvent>();
        services.Configure<MongoDbConfig>(configuration.GetSection(nameof(MongoDbConfig)));
        return services;
    }
}
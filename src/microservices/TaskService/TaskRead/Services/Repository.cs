using Common.Domain.Entities;
using Infrastructure.Config;
using Infrastructure.EventSourcing;
using Microsoft.Extensions.Options;

namespace TaskRead.Services;

public class TaskMongoRepository(IOptions<MongoDbConfig> config, ILogger<MongoDbEventStoreRepository> logger)
    : MongoEntityRepository<Ticket>(config, logger);
    
public class ProjectMongoRepository(IOptions<MongoDbConfig> config, ILogger<MongoDbEventStoreRepository> logger)
    : MongoEntityRepository<Project>(config, logger);
    
    
public class EpicMongoRepository(IOptions<MongoDbConfig> config, ILogger<MongoDbEventStoreRepository> logger)
    : MongoEntityRepository<Common.Domain.Entities.Epic>(config, logger);
    

using Common.Domain.Entities;
using Infrastructure.Config;
using Infrastructure.EventSourcing;
using Microsoft.Extensions.Options;
using Task = Common.Domain.Entities.Task;
using Epic = Common.Domain.Entities.Epic;

namespace TaskRead.Services;

public class TaskMongoRepository(IOptions<MongoDbConfig> config, ILogger<MongoDbEventStoreRepository> logger)
    : MongoEntityRepository<Task>(config, logger);
    
public class ProjectMongoRepository(IOptions<MongoDbConfig> config, ILogger<MongoDbEventStoreRepository> logger)
    : MongoEntityRepository<Project>(config, logger);
    
    
public class EpicMongoRepository(IOptions<MongoDbConfig> config, ILogger<MongoDbEventStoreRepository> logger)
    : MongoEntityRepository<Common.Domain.Entities.Epic>(config, logger);
    

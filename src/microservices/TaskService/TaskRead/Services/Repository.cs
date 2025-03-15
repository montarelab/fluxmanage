using System.Linq.Expressions;
using Common.Config;
using Common.Domain.Entities;
using Infrastructure.EventSourcing;
using Microsoft.Extensions.Options;

namespace TaskRead.Services;

public class TicketMongoRepository(IOptions<MongoDbConfig> config, ILogger<MongoEntityRepository<Ticket>> logger)
    : MongoEntityRepository<Ticket>(config, logger);

public class ProjectMongoRepository(IOptions<MongoDbConfig> config, ILogger<MongoEntityRepository<Project>> logger)
    : MongoEntityRepository<Project>(config, logger);

public class EpicMongoRepository(IOptions<MongoDbConfig> config, ILogger<MongoEntityRepository<Epic>> logger)
    : MongoEntityRepository<Epic>(config, logger);

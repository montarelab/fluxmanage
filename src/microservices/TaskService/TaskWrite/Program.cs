using System.Reflection;
using Common.Auth;
using Common.EventSourcing;
using Confluent.Kafka;
using FastEndpoints;
using FluentValidation;
using Infrastructure;
using Infrastructure.EventSourcing;
using Infrastructure.Middleware;
using Infrastructure.MongoDb;
using Infrastructure.Swagger;

var builder = WebApplication.CreateBuilder(args);

var configPath = Environment.GetEnvironmentVariable("APP_CONFIG_PATH");
builder.Configuration.AddJsonFile(configPath!, optional: false, reloadOnChange: true);
builder.Services.AddMongoDb(builder.Configuration);

builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));

var assembly = Assembly.GetExecutingAssembly();

builder.Services
    .AddScoped<ICurrentUserService, CurrentUserService>()
    .AddScoped<IEventStoreRepository, MongoDbEventStoreRepository>()
    .AddScoped<IEventProducer, EventProducer>()
    .AddScoped<IEventStore, EventStore>()
    .AddScoped(typeof(IEventSourcingHandler<>), typeof(EventSourcingHandler<>))
    .AddScoped<ExceptionMiddleware>();
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger("Write Task Api");
builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseSwaggerUtils();
app.UseFastEndpoints();
app.UseHttpsRedirection();
app.UseInfrastructure();

app.MapGet("/health", () => "Healthy");
app.Run();
using System.Reflection;
using Common.Events.Models;
using Confluent.Kafka;
using FastEndpoints;
using FluentValidation;
using Infrastructure;
using Infrastructure.Config;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

var builder = WebApplication.CreateBuilder(args);

var configPath = Environment.GetEnvironmentVariable("APP_CONFIG_PATH");
builder.Configuration.AddJsonFile(configPath!, optional: false, reloadOnChange: true);

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
BsonClassMap.RegisterClassMap<ProjectCreatedEvent>();
BsonClassMap.RegisterClassMap<ProjectUpdatedEvent>();
BsonClassMap.RegisterClassMap<ProjectDeletedEvent>();

BsonClassMap.RegisterClassMap<TaskCreatedEvent>();
BsonClassMap.RegisterClassMap<TaskDeletedEvent>();
BsonClassMap.RegisterClassMap<TaskUpdatedEvent>();

BsonClassMap.RegisterClassMap<EpicCreatedEvent>();
BsonClassMap.RegisterClassMap<EpicUpdatedEvent>();
BsonClassMap.RegisterClassMap<EpicDeletedEvent>();

// add configs
builder.Services.Configure<MongoDbConfig>(builder.Configuration.GetSection(nameof(MongoDbConfig)));
builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));

var assembly = Assembly.GetExecutingAssembly();

builder.Services.AddInfrastructure();
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFastEndpoints();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseFastEndpoints();
app.UseHttpsRedirection();
app.UseInfrastructure();

app.MapGet("/health", () => "Healthy");
app.Run();
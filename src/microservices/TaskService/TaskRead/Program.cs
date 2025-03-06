using System.Reflection;
using Common.Domain.Entities;
using Confluent.Kafka;
using FastEndpoints;
using FastEndpoints.Swagger;
using FluentValidation;
using Infrastructure;
using Infrastructure.MongoDb;
using Infrastructure.Swagger;
using TaskRead.Services;
using Task = Common.Domain.Entities.Task;

var builder = WebApplication.CreateBuilder(args);

var configPath = Environment.GetEnvironmentVariable("APP_CONFIG_PATH");
builder.Configuration.AddJsonFile(configPath!, optional: false, reloadOnChange: true);
builder.Services.AddMongoDb(builder.Configuration);

builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));

var assembly = Assembly.GetExecutingAssembly();

builder.Services.AddScoped<IRepository<Task>, TaskMongoRepository>();
builder.Services.AddScoped<IRepository<Epic>, EpicMongoRepository>();
builder.Services.AddScoped<IRepository<Project>, ProjectMongoRepository>();

builder.Services.AddInfrastructure();
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddFastEndpoints().SwaggerDocument();

var app = builder.Build();

app.UseSwaggerUtils();
app.UseFastEndpoints();
app.UseHttpsRedirection();
app.UseInfrastructure();

app.MapGet("/health", () => "Healthy");
app.Run();
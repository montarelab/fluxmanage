using System.Reflection;
using Confluent.Kafka;
using FastEndpoints;
using FastEndpoints.Swagger;
using FluentValidation;
using Infrastructure;
using Infrastructure.MongoDb;
using Infrastructure.Swagger;

var builder = WebApplication.CreateBuilder(args);

var configPath = Environment.GetEnvironmentVariable("APP_CONFIG_PATH");
builder.Configuration.AddJsonFile(configPath!, optional: false, reloadOnChange: true);
builder.Services.AddMongoDb(builder.Configuration);

builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));

var assembly = Assembly.GetExecutingAssembly();

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
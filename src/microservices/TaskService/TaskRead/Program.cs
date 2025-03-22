using System.Reflection;
using Common.Domain.Entities;
using Confluent.Kafka;
using FastEndpoints;
using FastEndpoints.Swagger;
using FluentValidation;
using Infrastructure;
using Infrastructure.MongoDb;
using Infrastructure.Swagger;
using TaskRead.KafkaConsumer;
using TaskRead.Services;

var builder = WebApplication.CreateBuilder(args);

var configPath = Environment.GetEnvironmentVariable("APP_CONFIG_PATH");
builder.Configuration.AddJsonFile(configPath!, optional: false, reloadOnChange: true);
builder.Services.Configure<ConsumerConfig>(builder.Configuration.GetSection(nameof(ConsumerConfig)));
builder.Services.AddMongoDb(builder.Configuration);

builder.Services.Configure<ProducerConfig>(builder.Configuration.GetSection(nameof(ProducerConfig)));

var assembly = Assembly.GetExecutingAssembly();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.WithOrigins("http://localhost:3000") // Allow frontend URL
                .AllowAnyHeader()
                .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IRepository<Ticket>, TicketMongoRepository>();
builder.Services.AddScoped<IRepository<Epic>, EpicMongoRepository>();
builder.Services.AddScoped<IRepository<Project>, ProjectMongoRepository>();
builder.Services.AddScoped<IUniversalEventHandler, UniversalEventHandler>();
builder.Services.AddScoped<IEventConsumer, EventConsumer>();
builder.Services.AddHostedService<ConsumerHostedService>();

builder.Services.AddInfrastructure();
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger("Read Task Api");
builder.Services.AddFastEndpoints().SwaggerDocument();

var app = builder.Build();

app.UseCors("AllowFrontend"); // Ensure this comes before UseRouting()
app.UseSwaggerUtils();
app.UseFastEndpoints();
app.UseHttpsRedirection();
app.UseInfrastructure();

app.MapGet("/health", () => "Healthy");
app.Run();
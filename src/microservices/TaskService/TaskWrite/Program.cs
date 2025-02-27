using System.Reflection;
using FastEndpoints;
using FluentValidation;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var assembly = Assembly.GetExecutingAssembly();

builder.Services.AddEventSourcingInfrastructure();
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddFastEndpoints();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseFastEndpoints();
app.UseHttpsRedirection();
app.Run();
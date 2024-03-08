using ConsumerAPI.Configuration;
using ConsumerAPI.Models;
using ConsumerAPI.Services.Hosted;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Configuration
    .AddJsonFile("appsettings.json", false, reloadOnChange: true)
    .AddUserSecrets<ConsumerConfiguration>(true)
    .AddEnvironmentVariables()
    .Build();

var services = builder.Services;

var config = builder.Configuration.Get<ConsumerConfiguration>() ?? throw new NullReferenceException("Could not get an instance of the configuration");

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddSingleton(new ConsumerKafkaSettings
{
    Topic = config.Kafka.Topic,
    BootstrapServers = config.Kafka.BootstrapServers,
    SaslUsername = config.Kafka.SaslUsername,
    SaslPassword = config.Kafka.SaslPassword,
    ConsumerGroup = config.Identifier
});

services.AddHostedService<KafkaListenerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", (context) =>
{
    return context.Response.WriteAsync("Hello from Consumer API");
});

app.Run();

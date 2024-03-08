using Common.Shared.Model;
using Microsoft.AspNetCore.Mvc;
using ProducerAPI.Configuration;
using ProducerAPI.Model;
using ProducerAPI.Services;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

builder.Configuration
    .AddJsonFile("appsettings.json", false, reloadOnChange: true)
    .AddUserSecrets<ProducerConfiguration>(true)
    .AddEnvironmentVariables()
    .Build();

var config = builder.Configuration.Get<ProducerConfiguration>() ?? throw new NullReferenceException("Could not get an instance of the configuration");

services.AddSingleton(new ProducerKafkaSettings
{
    Topic = config.Kafka.Topic,
    BootstrapServers = config.Kafka.BootstrapServers,
    SaslUsername = config.Kafka.SaslUsername,
    SaslPassword = config.Kafka.SaslPassword,
});

services.AddSingleton<KafkaProducer>();

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
    return context.Response.WriteAsync("Hello from Producer API");
});

app.MapPost("/kafka/produce", async ([FromBody] ProduceMessageRequest request, KafkaProducer kafkaProducer) =>
{
    await kafkaProducer.SendMessage(new KafkaMessage
    {
        Id = Guid.NewGuid(),
        MessageDelayInSeconds = request.MessageDelayInSeconds,
        Message = request.Message,
        RequestId = Guid.NewGuid(),
    });

    return Results.Ok();
})
.WithName("ProduceMessage")
.WithOpenApi();

app.MapPost("/kafka/producemany", async ([FromBody] ProduceManyMessagesRequest request, KafkaProducer kafkaProducer) =>
{
    var requestId = Guid.NewGuid();

    for (int i = 0; i < request.MessageCount; i++)
    {
        await kafkaProducer.SendMessage(new KafkaMessage
        {
            Id = Guid.NewGuid(),
            MessageDelayInSeconds = request.MessageDelayInSeconds,
            Message = $"Message: {i}",
            RequestId = requestId,
        });
    }

    return Results.Ok();
})
.WithName("ProduceListOfMessages")
.WithOpenApi();

app.Run();

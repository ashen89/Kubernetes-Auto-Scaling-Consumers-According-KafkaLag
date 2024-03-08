
using Common.Shared.Model;
using Confluent.Kafka;
using ConsumerAPI.Models;
using System.Text.Json;

namespace ConsumerAPI.Services.Hosted;

public class KafkaListenerService : BackgroundService
{
    private readonly IHostApplicationLifetime lifetime;
    private readonly ConsumerKafkaSettings settings;

    private readonly ConsumerConfig config;

    public KafkaListenerService(IHostApplicationLifetime lifetime, ConsumerKafkaSettings settings)
    {
        this.lifetime = lifetime;
        this.settings = settings;
        config = new ConsumerConfig
        {
            BootstrapServers = settings.BootstrapServers,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = settings.SaslUsername,
            SaslPassword = settings.SaslPassword,
            SocketTimeoutMs = 60000,
            Acks = Acks.Leader,
            GroupId = settings.ConsumerGroup
        };
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        lifetime.ApplicationStarted.Register(() => Task.Run(async () =>
        {
            using var c = new ConsumerBuilder<Ignore, string>(config).Build();
            c.Subscribe(settings.Topic);

            try
            {
                while (true)
                {
                    try
                    {
                        var cr = c.Consume(stoppingToken);

                        if (cr == null) continue;

                        var message = JsonSerializer.Deserialize<KafkaMessage>(cr.Message.Value);

                        if (cr.Message == null)
                            continue;

                        Console.WriteLine($"Consumed message '{cr.Message.Value}' at: '{cr.TopicPartitionOffset}'.");

                        if (message?.MessageDelayInSeconds > 0)
                        {
                            Console.WriteLine("Delaying Message Consume by {0} secs", message.MessageDelayInSeconds);
                            Console.WriteLine(string.Empty);
                            await Task.Delay(message.MessageDelayInSeconds * 1000, stoppingToken);
                        }
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occured: {e.Error.Reason}");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                c.Close();
            }
        }, stoppingToken));

        return Task.CompletedTask;
    }
}

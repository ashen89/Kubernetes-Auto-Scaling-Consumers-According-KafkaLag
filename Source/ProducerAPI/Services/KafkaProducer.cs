using Common.Shared.Model;
using Confluent.Kafka;
using ProducerAPI.Model;
using System.Text.Json;

namespace ProducerAPI.Services;

public class KafkaProducer
{
    private readonly ProducerKafkaSettings settings;
    private readonly ProducerConfig config;

    private IProducer<Null, string>? producer;

    private IProducer<Null, string> Producer
    {
        get
        {
            producer ??= new ProducerBuilder<Null, string>(config).Build();
            return producer;
        }
    }

    public KafkaProducer(ProducerKafkaSettings settings)
    {
        this.settings = settings;

        config = new ProducerConfig
        {
            BootstrapServers = settings.BootstrapServers,
            SecurityProtocol = SecurityProtocol.SaslSsl,
            SaslMechanism = SaslMechanism.Plain,
            SaslUsername = settings.SaslUsername,
            SaslPassword = settings.SaslPassword,
            SocketTimeoutMs = 60000,
            Acks = Acks.Leader
        };
    }

    public async Task SendMessage<T>(T message) where T : KafkaMessage
    {

        try
        {
            var dr = await Producer.ProduceAsync(settings.Topic, new Message<Null, string> { Value = JsonSerializer.Serialize(message) });
            Console.WriteLine($"Delivered '{dr.Value}' to '{dr.TopicPartitionOffset}'");
        }
        catch (ProduceException<Null, string> e)
        {
            Console.WriteLine($"Delivery failed: {e.Error.Reason}");
        }
    }
}



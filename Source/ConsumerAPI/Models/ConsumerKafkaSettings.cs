using Common.Shared.Model;

namespace ConsumerAPI.Models;

public class ConsumerKafkaSettings : KafkaSettings
{
    public required string ConsumerGroup { get; set; }
}

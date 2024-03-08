namespace Common.Shared.Configuration;

public class BaseConfiguration
{
    public required KafkaConfiguration Kafka { get; set; }
    public required string Identifier { get; set; }
}

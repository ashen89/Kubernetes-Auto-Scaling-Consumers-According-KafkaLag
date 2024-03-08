namespace Common.Shared.Configuration;

public class KafkaConfiguration
{
    public required string BootstrapServers { get; set; }
    public required string Topic { get; set; }
    public required string SaslUsername { get; set; }
    public required string SaslPassword { get; set; }
}

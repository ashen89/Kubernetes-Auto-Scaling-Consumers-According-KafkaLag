namespace Common.Shared.Model;

public class KafkaMessage
{
    public required Guid Id { get; set; }
    public required string Message { get; set; }
    public int MessageDelayInSeconds { get; set; } = 0;
    public required Guid RequestId { get; set; }
}

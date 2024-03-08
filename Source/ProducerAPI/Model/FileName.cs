namespace ProducerAPI.Model;

public class ProduceMessageRequest
{
    public required string Message { get; set; }
    public required int MessageDelayInSeconds { get; set; } = 0;
}

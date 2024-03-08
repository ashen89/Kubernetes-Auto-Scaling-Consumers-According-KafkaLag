namespace ProducerAPI.Model;

public class ProduceManyMessagesRequest
{
    public int MessageCount { get; set; }
    public int MessageDelayInSeconds { get; set; } = 0;
}

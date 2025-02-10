using Confluent.Kafka;

public class IntegrationEvent
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string GroupId { get; set; } = "";
    public string Topic { get; set; }= "";
    public AutoOffsetReset AutoOffsetReset { get; set; } = AutoOffsetReset.Earliest;

}
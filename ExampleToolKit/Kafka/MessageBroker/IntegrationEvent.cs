using Confluent.Kafka;

public class IntegrationEvent : KafkData<string, EventData>
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string GroupId { get; set; } = "";
    public string Topic { get; set; }= "";
    public AutoOffsetReset AutoOffsetReset { get; set; } = AutoOffsetReset.Earliest;

     public IntegrationEvent(string key, EventData value) : base(key, value)
     {
     }
}

public class KafkData<TKey, TValue>
{
    public TKey Key { get; set; } = default!;
    public TValue Value { get; set; } = default!;

    public KafkData(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}

public class EventData
{
    public string Value { get; set; } = default!;
   
   public EventData(){}
}


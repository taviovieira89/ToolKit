using System.Text.Json;

public class DomainEventLog
{
    public Guid Id { get; private set; }
    public string EventType { get; private set; } = default!;
    public string EventData { get; private set; } = default!;
    public DateTime OccurredOn { get; private set; }
    public string AggregateId { get; private set; } = default!;
    public string AggregateName { get; private set; } = default!;
    public bool Processed { get; private set; }
    public DateTime? ProcessedOn { get; private set; }

    private DomainEventLog() { }

    public static DomainEventLog Create(object domainEvent, string aggregateId, string aggregateName)
    {
        return new DomainEventLog()
        {
            Id = Guid.NewGuid(),
            EventType = domainEvent.GetType().FullName!,
            EventData = JsonSerializer.Serialize(domainEvent),
            OccurredOn = DateTime.UtcNow,
            AggregateId = aggregateId,
            AggregateName = aggregateName,
            Processed = false
        };
    }

    public void MarkAsProcessed()
    {
        Processed = true;
        ProcessedOn = DateTime.UtcNow;
    }
}
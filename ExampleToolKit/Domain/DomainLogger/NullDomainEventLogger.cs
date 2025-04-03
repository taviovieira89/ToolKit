public class NullDomainEventLogger : IDomainEventLogger
{
    public Task LogEventAsync(object domainEvent, string aggregateId, string aggregateName)
    {
        return Task.CompletedTask;
    }

    public Task MarkEventAsProcessedAsync(object domainEvent)
    {
        return Task.CompletedTask;
    }

    public Task<IEnumerable<object>> GetEventsByAggregateIdAsync(string aggregateId, Type eventType = null!)
    {
        return Task.FromResult(Enumerable.Empty<object>());
    }

    public Task<IEnumerable<object>> GetUnprocessedEventsAsync(Type eventType = null!)
    {
        return Task.FromResult(Enumerable.Empty<object>());
    }
}
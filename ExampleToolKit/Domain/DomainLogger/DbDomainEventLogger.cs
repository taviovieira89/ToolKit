using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

public class DbDomainEventLogger : IDomainEventLogger
{
    private readonly DbContext _context;

    public DbDomainEventLogger(DbContext context)
    {
        _context = context;
    }

    public async Task LogEventAsync(object domainEvent, string aggregateId, string aggregateName)
    {
        var eventLog = DomainEventLog.Create(domainEvent, aggregateId, aggregateName);
        _context.Set<DomainEventLog>().Add(eventLog);
        await _context.SaveChangesAsync();
    }

    public async Task MarkEventAsProcessedAsync(object domainEvent)
    {
        var eventType = domainEvent.GetType().FullName;
        var eventData = JsonSerializer.Serialize(domainEvent);

        var eventLog = await _context.Set<DomainEventLog>()
            .FirstOrDefaultAsync(e => e.EventType == eventType &&
                                     e.EventData.Contains(eventData.Substring(0, Math.Min(100, eventData.Length))) &&
                                     !e.Processed);

        if (eventLog != null)
        {
            eventLog.MarkAsProcessed();
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<object>> GetEventsByAggregateIdAsync(string aggregateId, Type eventType = null!)
    {
        var query = _context.Set<DomainEventLog>()
            .Where(e => e.AggregateId == aggregateId);

        if (eventType != null)
        {
            query = query.Where(e => e.EventType == eventType.FullName);
        }

        var logs = await query.OrderBy(e => e.OccurredOn).ToListAsync();

        return logs.Select(log =>
        {
            var type = Type.GetType(log.EventType);
            if (type == null) return null;

            try
            {
                return JsonSerializer.Deserialize(log.EventData, type);
            }
            catch
            {
                return null;
            }
        }).Where(e => e != null)!;
    }

    public async Task<IEnumerable<object>> GetUnprocessedEventsAsync(Type eventType = null!)
    {
        var query = _context.Set<DomainEventLog>()
            .Where(e => !e.Processed);

        if (eventType != null)
        {
            query = query.Where(e => e.EventType == eventType.FullName);
        }

        var logs = await query.OrderBy(e => e.OccurredOn).ToListAsync();

        return logs.Select(log =>
        {
            var type = Type.GetType(log.EventType);
            if (type == null) return null;

            try
            {
                return JsonSerializer.Deserialize(log.EventData, type);
            }
            catch
            {
                return null;
            }
        }).Where(e => e != null)!;
    }
}
using MediatR;
using Microsoft.EntityFrameworkCore;

public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    private readonly TContext _context;
    private readonly MongoDbContext _mongoContext;
    private readonly IMediator _mediator;
    private readonly Dictionary<Type, object> _repositories = new();
    private bool _disposed = false;

    private readonly IDomainEventLogger _eventLogger;

    public UnitOfWork(
        TContext context,
        IMediator mediator,
        MongoDbContext mongoContext = null!,
        IDomainEventLogger eventLogger = null!)
    {
        _context = context;
        _mediator = mediator;
        _mongoContext = mongoContext;
        _eventLogger = eventLogger ?? new NullDomainEventLogger();
    }

    public IRepository<T, TContext> Repository<T>(bool useMongo = false) where T : class
    {
        if (!_repositories.ContainsKey(typeof(T)))
        {
            object repository = useMongo
                ? new MongoRepository<T>(_mongoContext, typeof(T).Name)
                : new Repository<T, TContext>(_context); // Passamos o contexto correto

            _repositories[typeof(T)] = repository;
        }

        return (IRepository<T, TContext>)_repositories[typeof(T)];
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();

        var aggregateRoots = _context.ChangeTracker
            .Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        var domainEvents = aggregateRoots
            .SelectMany(e => e.DomainEvents)
            .ToList();

        // Registramos cada evento no log antes de publicá-lo
        foreach (var aggregateRoot in aggregateRoots)
        {
            foreach (var domainEvent in aggregateRoot.DomainEvents)
            {
                await _eventLogger.LogEventAsync(
                    domainEvent,
                    aggregateRoot.GetId().ToString()!,
                    aggregateRoot.GetType().Name
                );
            }
        }

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent);
            await _eventLogger.MarkEventAsProcessedAsync(domainEvent);
        }

        aggregateRoots.ForEach(e => e.ClearDomainEvents());
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

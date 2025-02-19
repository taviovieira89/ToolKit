using MediatR;
using Microsoft.EntityFrameworkCore;

public class UnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    private readonly TContext _context;
    private readonly MongoDbContext _mongoContext;
    private readonly IMediator _mediator;
    private readonly Dictionary<Type, object> _repositories = new();
    private bool _disposed = false;

    public UnitOfWork(
        TContext context, 
        IMediator mediator, 
        MongoDbContext mongoContext = null!)
    {
        _context = context;
        _mediator = mediator;
        _mongoContext = mongoContext;
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
            .ToList();

        var domainEvents = aggregateRoots
            .SelectMany(e => e.DomainEvents)
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent);
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

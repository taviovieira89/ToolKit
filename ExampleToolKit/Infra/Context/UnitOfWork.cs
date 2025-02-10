using MediatR;

// Implementação da Unidade de Trabalho (Unit of Work)
public class UnitOfWork : IUnitOfWork
{
    private readonly ContextDb _context;
    private readonly MongoDbContext _mongoContext;
    private readonly IMediator _mediator;
    private readonly Dictionary<Type, object> _repositories = new();
    private bool _disposed = false;

    public UnitOfWork(
    ContextDb context, 
    IMediator mediator, 
    MongoDbContext mongoContext)
    {
        _context = context;
        _mediator = mediator;
        _mongoContext = mongoContext;
    }

    public IRepository<T> Repository<T>(bool useMongo = false) where T : class
    {
         if (!_repositories.ContainsKey(typeof(T)))
        {
            object repository = useMongo
                ? new MongoRepository<T>(_mongoContext, typeof(T).Name)
                : new Repository<T>(_context);

            _repositories[typeof(T)] = repository;
        }

        return (IRepository<T>)_repositories[typeof(T)];
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

        // Agora estamos limpando os eventos de domínio corretamente!
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

//Exemplo de Uso de Db Sql server ou MongoDB
// var sqlContext = new ContextDb(...);  // SQL Server
// var mongoContext = new MongoDbContext("mongodb://localhost:27017", "MinhaBase");
// var unitOfWork = new UnitOfWork(sqlContext, mongoContext);

// // Criando um repositório no SQL Server
// var sqlRepo = unitOfWork.Repository<Produto>();
// sqlRepo.Add(new Produto { Nome = "Produto SQL" });
// unitOfWork.SaveChanges();

// // Criando um repositório no MongoDB
// var mongoRepo = unitOfWork.Repository<Produto>(useMongo: true);
// mongoRepo.Add(new Produto { Nome = "Produto MongoDB" });

// var produtosNoMongo = await mongoRepo.GetAllAsync();
// Console.WriteLine($"Total de produtos no MongoDB: {produtosNoMongo.Count()}");
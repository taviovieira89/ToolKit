using MongoDB.Driver;

public class MongoRepository<T> : IMongoDbRepository<T> where T : class
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(MongoDbContext context, string collectionName)
    {
        _collection = context.GetCollection<T>(collectionName);
    }

    public void Add(T entity)
    {
        _collection.InsertOne(entity);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public IEnumerable<T> GetAll()
    {
        return _collection.Find(_ => true).ToList();
    }
}

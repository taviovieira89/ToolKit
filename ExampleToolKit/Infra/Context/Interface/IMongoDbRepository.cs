public interface IMongoDbRepository<T> where T : class
{
    void Add(T entity);
    IEnumerable<T> GetAll();
    Task<IEnumerable<T>> GetAllAsync();
}

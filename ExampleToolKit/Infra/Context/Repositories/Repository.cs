using Microsoft.EntityFrameworkCore;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly ContextDb _context;
    private readonly DbSet<T> _dbSet;

    public Repository(ContextDb context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public IEnumerable<T> GetAll()
    {
        return _dbSet.ToList();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }
}
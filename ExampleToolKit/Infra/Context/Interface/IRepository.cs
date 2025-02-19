// Interface para repositórios genéricos
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
public interface IRepository<T,TContext> 
where T : class
where TContext : DbContext
{
    void Add(T entity);
    IEnumerable<T> GetAll();
    Task<IEnumerable<T>> GetAllAsync();

    IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    // Novo método para buscar pelo ID
    T GetById(object id);

    Task<T> GetByIdAsync(object id);

}

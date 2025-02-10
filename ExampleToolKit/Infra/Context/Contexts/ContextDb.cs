using Microsoft.EntityFrameworkCore;

// Implementação do DbContext
public class ContextDb : DbContext
{
    public ContextDb(DbContextOptions<ContextDb> options) : base(options)
    {
    }

    // public DbSet<T> SetEntity<T>() where T : class
    // {
    //     return Set<T>();
    // }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
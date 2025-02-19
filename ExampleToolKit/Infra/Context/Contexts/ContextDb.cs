using Microsoft.EntityFrameworkCore;

public class ContextDb<T> : DbContext where T : DbContext
{
    public ContextDb(DbContextOptions<T> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}

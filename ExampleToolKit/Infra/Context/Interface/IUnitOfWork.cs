// Interface para Unidade de Trabalho (Unit of Work)
public interface IUnitOfWork : IDisposable
{
    void SaveChanges();
    Task SaveChangesAsync();
}

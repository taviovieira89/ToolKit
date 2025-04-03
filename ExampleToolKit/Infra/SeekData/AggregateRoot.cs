using MediatR;

public abstract class AggregateRoot : IEntity
{
    // Propriedade Id abstrata que deve ser implementada por todas as classes derivadas
    public abstract object GetId();
    private readonly List<INotification> _domainEvents = new();

    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
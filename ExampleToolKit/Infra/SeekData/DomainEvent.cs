using MediatR;

public class DomainEvent : INotification
{
    public string EntityName { get; }

    public DomainEvent(string entityName)
    {
        EntityName = entityName;
    }
}
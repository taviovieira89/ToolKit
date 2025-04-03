public abstract class Entity<TId> : AggregateRoot
{
    public TId Id { get; protected set; } = default!;
    
    public override object GetId()
    {
        return Id!;
    }
}

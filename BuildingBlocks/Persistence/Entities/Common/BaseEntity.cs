using System;
namespace BuildingBlocks.Persistence.Entities.Common;

public abstract class BaseEntity : DomainEventEntity
{
    public int Id { get; set; }
}
public abstract class BaseEntity<TKey> : DomainEventEntity
{
    public virtual TKey Id { get; protected set; }

    protected BaseEntity()
    {
    }

    protected BaseEntity(TKey id)
    {
        Id = id;
    }
}

using MediatR;

namespace BuildingBlocks.Persistence.Abstractions.Event;

public interface IEvent: INotification
{
    Guid EventId => Guid.NewGuid();
    public DateTime OccurredOn => DateTime.Now;
    public string EventType => GetType().AssemblyQualifiedName;
}


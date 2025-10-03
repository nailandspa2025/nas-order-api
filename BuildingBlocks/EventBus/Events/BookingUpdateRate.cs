namespace BuildingBlocks.EventBus.Events;

public class BookingUpdateRateEvent
{
    public int BookingId { get; set; }

    public bool IsRated { get; set; }
}


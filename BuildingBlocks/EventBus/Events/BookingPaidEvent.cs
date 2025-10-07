namespace BuildingBlocks.EventBus.Events;

public class BookingPaidEvent
{
    public long BookingId { get; set; }

    public string AccountId { get; set; } = null!;

    public long StoreId { get; set; }

    public decimal Amount { get; set; }

    public int Process { get; set; }

    public int MerchantId { get; set; }
}

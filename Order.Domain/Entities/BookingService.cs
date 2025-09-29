namespace Order.Domain.Entities;

public class BookingService
{
    public int Id { get; set; }

    public int ServiceId { get; set; }

    public Booking? Booking { get; set; }
}

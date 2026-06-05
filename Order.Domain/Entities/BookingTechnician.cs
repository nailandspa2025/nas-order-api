namespace Order.Domain.Entities;

public class BookingTechnician
{
    public int Id { get; set; }
    public long TechnicianId { get; set; }
    public int BookingId { get; set; }
    public Booking Booking { get; set; } = null!;
    public ICollection<BookingTechnicianService> Services { get; set; }
        = new List<BookingTechnicianService>();
}

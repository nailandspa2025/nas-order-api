namespace Order.Domain.Entities;

public class BookingTechnicianService
{
    public int Id { get; set; }

    public int BookingTechnicianId { get; set; }

    public int ServiceId { get; set; }

    public BookingTechnician BookingTechnician { get; set; } = null!;
}
using BuildingBlocks.Persistence.Entities.Common;

namespace Order.Domain.Entities;

public class BookingReminderLog : BaseAuditableEntity<int>
{
    public int BookingId { get; set; }
    public int ReminderConfigId { get; set; }
    public ReminderChannel Channel { get; set; }
    public DateTime SentAt { get; set; }
    public string? Error { get; set; }
    public Booking Booking { get; set; }
    public ReminderConfig ReminderConfig { get; set; }
}
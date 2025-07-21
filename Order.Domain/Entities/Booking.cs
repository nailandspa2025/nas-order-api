using Order.Domain.Enums;
using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Order.Domain.Entities;

public class Booking : BaseAuditableEntity<int>, ISoftDelete
{
    public long ? StoreId { get; set; }

    public long ? ProductId { get; set; }

    //public long ? TechnicianId { get; set; }

    public string ? UserId { get; set; } = null!;

    public DateTime BookingDate { get; set; }

    public TimeSpan BookingTime { get; set; }

    public BookingStatus Status { get; set; }

    public PaymentStatus ? PaymentStatus { get; set; }

    public PaymentMethod ? PaymentMethod { get; set; }

    public string ? Note { get; set; }

    public string ? FullName { get; set; }

    public string ? Address { get; set; }

    public Gender ? Gender { get; set; }

    public string ? Phone { get; set; }

    public string ? Email { get; set; }

    public int ? Number { get; set; }

    public int TransactionId { get; set; }

    public virtual Payment? Payment { get; set; }
    
    public string? Reason { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public bool IsDeleted { get; set; }

    public int? BookingCancelReasonId { get; set; } 

    public virtual BookingCancelReason? BookingCancelReason { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public string? SnapId { get; set; }
    public string? GroupdId { get; set; }
    public ICollection<BookingService> BookingServices { get; set; } = new List<BookingService>();
    public ICollection<BookingTechnician> BookingTechnicians { get; set; } = new HashSet<BookingTechnician>();

    public void SetBookingServices(List<BookingService> bookingServices)
    {
        this.BookingServices.Clear();
        this.BookingServices = bookingServices;
    }

    public void SetBookingTechnicians(List<BookingTechnician> bookingTechnicians)
    {
        this.BookingTechnicians.Clear();
        this.BookingTechnicians = bookingTechnicians;
    }
}
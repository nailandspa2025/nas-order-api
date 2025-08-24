using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;
namespace Order.Domain.Entities;

public class Payment : BaseAuditableEntity<int>, ISoftDelete
{
    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? PaidAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public  PaymentMethod Method { get; set; }

    public PaymentStatus Status { get; set; }

    public string? TransactionId { get; set; } 

    public string? PaymentUrl { get; set; } 

    public string? Description { get; set; }

    public string? FullName { get; init; }

    public string? Email { get; init; }

    public string? Phone { get; init; }

    public string? DeletedBy { get; set; }

    public DateTime? Deleted { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    public string? ApproveUrl { get; set; }

}
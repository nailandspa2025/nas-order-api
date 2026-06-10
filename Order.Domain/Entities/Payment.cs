using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;
namespace Order.Domain.Entities;

public class Payment : BaseAuditableEntity<int>, ISoftDelete
{
    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? PaidAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public PaymentMethod Method { get; set; }

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
    
    // Tổng tiền dịch vụ
    public decimal ServiceAmount { get; set; }

    // Giảm giá
    public decimal DiscountAmount { get; set; }

    // Phụ thu
    public decimal SurchargeAmount { get; set; }

    // Tiền khách đưa (tiền mặt)
    public decimal? CustomerPaid { get; set; }

    // Tiền thối lại
    public decimal? ChangeAmount { get; set; }

}
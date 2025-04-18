using BuildingBlocks.Persistence.Entities.Common;

namespace Order.Domain.Entities;

public class Transaction : BaseAuditableEntity<int>
{
    public int PaymentId { get; set; }

    public string TransactionId { get; set; } = null!;

    public decimal Amount { get; set; }

    public string Provider { get; set; } = null!;

    public TransactionStatus Status { get; set; }

    public DateTime ProcessedAt { get; set; }

    public virtual Payment Payment { get; set; } = null!;
}


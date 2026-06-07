
using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Order.Domain.Entities;

public class PaymentProvider : BaseAuditableEntity<int>, ISoftDelete
{
    public PaymentMethod PaymentMethod { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? Deleted { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<PaymentProviderSetting> PaymentProviderSettings { get; set; }
        = new List<PaymentProviderSetting>();
    public void SetPaymentProviderSettings(List<PaymentProviderSetting> settings)
    {
        this.PaymentProviderSettings.Clear();
        this.PaymentProviderSettings = settings;
    }
}
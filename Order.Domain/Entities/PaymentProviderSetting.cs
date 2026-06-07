
using BuildingBlocks.Persistence.Abstractions.Entities;
using BuildingBlocks.Persistence.Entities.Common;

namespace Order.Domain.Entities;

public class PaymentProviderSetting : BaseAuditableEntity<int>
{
    public string Key { get; set; } = null!;
    public string Value { get; set; } = null!;
    public bool IsEncrypted { get; set; }
    public int PaymentProviderId { get; set; }
    public PaymentProvider PaymentProvider { get; set; } = null!;
}
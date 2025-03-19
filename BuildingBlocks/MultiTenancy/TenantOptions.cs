using BuildingBlocks.MultiTenancy.Abstractions;

namespace BuildingBlocks.MultiTenancy;

public class TenantOptions
{
    public string? DefaultConnection { get; set; }
    public TenantConfiguration[] TenantConfigurations { get; set; } = Array.Empty<TenantConfiguration>();
}


namespace BuildingBlocks.MultiTenancy.Abstractions;

public class BasicTenantInfo
{
    public Guid TenantId { get; }

    public BasicTenantInfo(Guid tenantId)
    {
        TenantId = tenantId;
    }
}


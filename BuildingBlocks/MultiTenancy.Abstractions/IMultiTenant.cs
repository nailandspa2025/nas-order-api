namespace BuildingBlocks.MultiTenancy.Abstractions;

public interface IMultiTenant
{
    Guid TenantId { get; }
}


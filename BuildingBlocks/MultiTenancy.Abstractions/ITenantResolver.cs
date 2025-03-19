namespace BuildingBlocks.MultiTenancy.Abstractions;

public interface ITenantResolver
{
    Guid TenantId { get; }
    TenantConfiguration GetCurrentTenantConfiguration();
    IDisposable Change(Guid id);
    Task<TenantConfiguration> GetCurrentTenantConfigurationAsync();
}


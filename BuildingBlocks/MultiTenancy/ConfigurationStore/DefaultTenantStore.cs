using BuildingBlocks.MultiTenancy.Abstractions;
using Microsoft.Extensions.Options;

namespace BuildingBlocks.MultiTenancy.ConfigurationStore;

public class DefaultTenantStore : ITenantStore
{
    private readonly TenantOptions _tenantOptions;

    public DefaultTenantStore(IOptions<TenantOptions> options)
    {
        _tenantOptions = options.Value;
    }

    public Task<TenantConfiguration[]> FindAllAsync()
    {
        return Task.FromResult(_tenantOptions.TenantConfigurations);
    }

    public Task<TenantConfiguration?> FindAsync(string name)
    {
        return Task.FromResult(Find(name));
    }

    public Task<TenantConfiguration?> FindAsync(Guid id)
    {
        return Task.FromResult(Find(id));
    }

    private TenantConfiguration? Find(string name)
    {
        return _tenantOptions.TenantConfigurations?.FirstOrDefault(t => t.Name == name);
    }

    private TenantConfiguration? Find(Guid id)
    {
        return _tenantOptions.TenantConfigurations?.FirstOrDefault(t => t.Id == id);
    }
}


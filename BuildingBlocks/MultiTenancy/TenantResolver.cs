using BuildingBlocks.MultiTenancy.Abstractions;

namespace BuildingBlocks.MultiTenancy;

public class TenantResolver: ITenantResolver
{
    private readonly IEnumerable<ITenantResolveContributor> _tenantResolveContributors;
    private readonly ITenantStore _tenantStore;
    private readonly ICurrentTenantAccessor _currentTenantAccessor;

    public TenantResolver(
        IEnumerable<ITenantResolveContributor> tenantResolveContributors,
        ITenantStore tenantStore,
        ICurrentTenantAccessor currentTenantAccessor)
    {
        _currentTenantAccessor = currentTenantAccessor;
        _tenantResolveContributors = tenantResolveContributors;
        _tenantStore = tenantStore;
    }

    public Guid TenantId => GetCurrentTenantConfiguration()?.Id ?? Guid.Empty;

    public TenantConfiguration GetCurrentTenantConfiguration()
    {
        return GetCurrentTenantConfigurationAsync().GetAwaiter().GetResult();
    }

    public async Task<TenantConfiguration> GetCurrentTenantConfigurationAsync()
    {
        string tenantIdValue = string.Empty;

        foreach (var tenantResolveContributors in _tenantResolveContributors)
        {
            tenantIdValue = await tenantResolveContributors.GetTenantIdFromHttpContextOrEmptyAsync();

            if (!string.IsNullOrWhiteSpace(tenantIdValue))
            {
                break;
            }
        }

        var tenants = await _tenantStore.FindAllAsync();
        var defaultTenant = tenants.FirstOrDefault(t => t.IsDefault());

        if (defaultTenant == null)
        {
            throw new ArgumentNullException(nameof(TenantConfiguration));
        }

        if (!Guid.TryParse(tenantIdValue, out Guid tenantId))
        {
            return defaultTenant;
        }

        var tenant = await _tenantStore.FindAsync(tenantId);

        if (tenant == null)
        {
            throw new ArgumentNullException(nameof(TenantConfiguration));
        }

        return tenant;
    }

    public IDisposable Change(Guid id)
    {
        return SetCurrent(id);
    }

    private IDisposable SetCurrent(Guid tenantId)
    {
        var parentScope = _currentTenantAccessor.Current;
        _currentTenantAccessor.Current = new BasicTenantInfo(tenantId);

        return new DisposeAction<ValueTuple<ICurrentTenantAccessor, BasicTenantInfo?>>(static (state) =>
        {
            var (currentTenantAccessor, parentScope) = state;
            currentTenantAccessor.Current = parentScope;
        }, (_currentTenantAccessor, parentScope));
    }
}


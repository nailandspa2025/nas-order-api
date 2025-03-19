using BuildingBlocks.MultiTenancy.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace BuildingBlocks.MultiTenancy.TenantResolveContributors;

public class RouteValueTenantResolveContributor: ITenantResolveContributor
{
    public const string ContributorName = "RouteValue";
    public const string Route = "tenantId";

    public string Name => ContributorName;

    private readonly IHttpContextAccessor _contextAccessor;

    public RouteValueTenantResolveContributor(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public Task<string> GetTenantIdFromHttpContextOrEmptyAsync()
    {
        if (_contextAccessor?.HttpContext == null)
        {
            return Task.FromResult(string.Empty);
        }

        var tenantValue = _contextAccessor.HttpContext.GetRouteValue(Route);

        if (tenantValue != null)
        {
            return Task.FromResult(tenantValue.ToString() ?? string.Empty);
        }

        return Task.FromResult(string.Empty);
    }
}


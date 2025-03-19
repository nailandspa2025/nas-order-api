using BuildingBlocks.MultiTenancy.Abstractions;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.MultiTenancy.TenantResolveContributors;

public class JwtTokenTenantResolveContributor: ITenantResolveContributor
{
    public const string ContributorName = "JwtToken";
    public const string JwtTokenClaimType = "tenant_id";

    public string Name => ContributorName;

    private readonly IHttpContextAccessor _contextAccessor;

    public JwtTokenTenantResolveContributor(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public Task<string> GetTenantIdFromHttpContextOrEmptyAsync()
    {
        if (_contextAccessor?.HttpContext == null)
        {
            return Task.FromResult(string.Empty);
        }

        var tenantValue = _contextAccessor.HttpContext?.User?.FindFirst(JwtTokenClaimType)?.Value ?? string.Empty;
        return Task.FromResult(tenantValue);
    }
}


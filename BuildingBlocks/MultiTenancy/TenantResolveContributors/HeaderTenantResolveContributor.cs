using BuildingBlocks.MultiTenancy.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.MultiTenancy.TenantResolveContributors;

public class HeaderTenantResolveContributor: ITenantResolveContributor
{
    public const string ContributorName = "Header";
    public const string HEADER_KEY = "X-Tenant-Id";

    public string Name => ContributorName;

    private readonly IHttpContextAccessor _contextAccessor;
    private readonly ILogger<HeaderTenantResolveContributor> _logger;

    public HeaderTenantResolveContributor(
        IHttpContextAccessor contextAccessor,
        ILogger<HeaderTenantResolveContributor> logger)
    {
        _contextAccessor = contextAccessor;
        _logger = logger;
    }

    public Task<string> GetTenantIdFromHttpContextOrEmptyAsync()
    {
        if (_contextAccessor?.HttpContext == null)
        {
            return Task.FromResult(string.Empty);
        }

        if (_contextAccessor?.HttpContext.Request.Headers == null || !_contextAccessor.HttpContext.Request.Headers.Any())
        {
            return Task.FromResult(string.Empty);
        }

        var tenantIdHeader = _contextAccessor.HttpContext.Request.Headers[HEADER_KEY];
        if (tenantIdHeader == string.Empty || tenantIdHeader.Count < 1)
        {
            return Task.FromResult(string.Empty);
        }

        if (tenantIdHeader.Count > 1)
        {
            _logger.LogWarning($"HTTP request includes more than one {HEADER_KEY} header value. First one will be used");
        }

        return Task.FromResult(tenantIdHeader.First() ?? string.Empty);
    }
}


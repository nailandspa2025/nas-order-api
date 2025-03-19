using BuildingBlocks.MultiTenancy.Abstractions;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.MultiTenancy.TenantResolveContributors;

public class QueryStringTenantResolveContributor: ITenantResolveContributor
{
    public const string ContributorName = "QueryString";
    public const string QueryStringKey = "tenantId";
    public string Name => ContributorName;

    private readonly IHttpContextAccessor _contextAccessor;

    public QueryStringTenantResolveContributor(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }

    public Task<string> GetTenantIdFromHttpContextOrEmptyAsync()
    {
        if (_contextAccessor?.HttpContext == null)
        {
            return Task.FromResult(string.Empty);
        }

        if (_contextAccessor.HttpContext.Request.QueryString.HasValue)
        {
            if (_contextAccessor.HttpContext.Request.Query.ContainsKey(QueryStringKey))
            {
                var tenantValue = _contextAccessor.HttpContext.Request.Query[QueryStringKey].ToString();
                if (string.IsNullOrWhiteSpace(tenantValue))
                {
                    return Task.FromResult(string.Empty);
                }

                return Task.FromResult(tenantValue);
            }
        }

        return Task.FromResult(string.Empty);
    }
}


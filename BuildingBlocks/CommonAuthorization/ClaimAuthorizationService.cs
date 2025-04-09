using BuildingBlocks.CommonAuthorization;
using Microsoft.AspNetCore.Http;

namespace BuildingBlocksCommonAuthorization;

public class ClaimAuthorizationService: IAuthorizationService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ClaimAuthorizationService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> HasPermissionAsync(string accessGroups)
    {
        if (string.IsNullOrWhiteSpace(accessGroups))
            return true;

        var accessList = _httpContextAccessor.HttpContext?.User?
            .FindAll("Accesses")
            .Select(c => c.Value.ToLower())
            .ToHashSet() ?? new HashSet<string>();
        if (accessList.Count() < 1) return false;
        var requiredPermissions = accessGroups.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var permission in requiredPermissions)
        {
            var parts = permission.Split('.', StringSplitOptions.RemoveEmptyEntries);
            var module = parts.Length > 0 ? parts[0] : string.Empty;
            var moduleAdmin = $"{module}.admin";

            bool hasAccess = accessList.Contains(permission.ToLower()) ||
                             accessList.Contains("admin") ||
                             accessList.Contains(moduleAdmin);

            if (!hasAccess)
                return false;
        }

        return true;
    }
}


using System.Security.Claims;
using BuildingBlocks.Authentication.Abstractions;
using Microsoft.AspNetCore.Http;
using Duende.IdentityModel;

namespace BuildingBlocks.Authentication;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public string? UserName
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email) ?? "system";
        }
    }

    public IEnumerable<string> Roles
    {
        get
        {
            return _httpContextAccessor?.HttpContext?.User?.FindAll(JwtClaimTypes.Role)?.Select(x => x.Value)?.ToList() ?? Enumerable.Empty<string>();
        }
    }
    public string? UserId
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString();
        }
    }

    public string? FullName
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue("fullname") ?? null;
        }
    }
}


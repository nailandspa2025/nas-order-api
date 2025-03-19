using System.Security.Claims;

namespace BuildingBlocks.Authentication.Abstractions;

public interface ICurrentUser
{
    ClaimsPrincipal? User { get; }
    string? UserId { get; }
    string? UserName { get; }
    IEnumerable<string> Roles { get; }
    Guid TenantId { get; }
}
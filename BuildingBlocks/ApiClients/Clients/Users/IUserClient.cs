using BuildingBlocks.ApiClients.Clients.Users.Models;
using BuildingBlocks.Core.Response;

namespace BuildingBlocks.ApiClients.Clients.Users;

public interface IUserClient
{
    [Refit.Get("/api/v1/Identities")]
    Task<ApiResponse<IEnumerable<UserDto>>> GetTenantsForRegisterAsync(CancellationToken cancellationToken = default);
}


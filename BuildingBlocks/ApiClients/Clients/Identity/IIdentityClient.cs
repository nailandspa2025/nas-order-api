using BuildingBlocks.ApiClients.Clients.AccountDevice.Models;
using BuildingBlocks.Core.Response;

namespace BuildingBlocks.ApiClients.Clients.Identity;

public interface IIdentityClient
{
    [Refit.Get("/api/v1/accountdevices/{AccountId}")]
    Task<ApiResponse<IEnumerable<AccountDeviceDto>>> GetAccountDeviceAsync(string AccountId, CancellationToken cancellationToken = default);
}



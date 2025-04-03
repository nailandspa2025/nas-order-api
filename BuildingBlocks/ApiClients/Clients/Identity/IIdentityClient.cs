using BuildingBlocks.ApiClients.Clients.AccountDevice.Models;
using BuildingBlocks.ApiClients.Clients.Identity.Technicians.Models;
using BuildingBlocks.Core.Response;

namespace BuildingBlocks.ApiClients.Clients.Identity;

public interface IIdentityClient
{
    [Refit.Get("/api/v1/accountdevices/{AccountId}")]
    Task<ApiResponse<IEnumerable<AccountDeviceDto>>> GetAccountDeviceAsync(string AccountId, CancellationToken cancellationToken = default);

    [Refit.Get("/api/v1/technicians/ids")]
    Task<ApiResponse<IEnumerable<TechnicianDto>>> GetTechnicianByIdsAsync(string ids, CancellationToken cancellationToken = default);

    [Refit.Get("/api/v1/technicians/{id}")]
    Task<ApiResponse<TechnicianDto>> GetTechnicianByIdAsync(long id, CancellationToken cancellationToken = default);
}



using BuildingBlocks.ApiClients.Clients.AccountDevice.Models;
using BuildingBlocks.ApiClients.Clients.Identity.Technicians.Models;
using BuildingBlocks.ApiClients.Clients.Identity.Users.Models;
using BuildingBlocks.Core.Response;

namespace BuildingBlocks.ApiClients.Clients.Identity;

public interface IIdentityClient
{
    [Refit.Get("/api/v1/accountdevices/by-account-ids")]
    Task<ApiResponse<IEnumerable<AccountDeviceDto>>> GetAccountDeviceAsync(string accountIds, CancellationToken cancellationToken = default);

    [Refit.Get("/api/v1/technicians/ids")]
    Task<ApiResponse<IEnumerable<TechnicianDto>>> GetTechnicianByIdsAsync(string ids, CancellationToken cancellationToken = default);

    [Refit.Get("/api/v1/technicians/mobile/{id}")]
    Task<ApiResponse<TechnicianDto>> GetTechnicianByIdAsync(long id, CancellationToken cancellationToken = default);

    [Refit.Get("/api/v1/AppAccounts/ids")]
    Task<ApiResponse<IEnumerable<AppAccountDto>>> GetAppAccountByIdsAsync(string ids, CancellationToken cancellationToken = default);

    [Refit.Get("/api/v1/accountdevices/by-store/{storeId}")]
    Task<ApiResponse<IEnumerable<AccountDeviceDto>>> GetAccountDeviceByStoreIdAsync(long storeId, CancellationToken cancellationToken = default);
}



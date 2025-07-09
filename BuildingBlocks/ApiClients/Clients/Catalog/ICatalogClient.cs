using BuildingBlocks.ApiClients.Clients.Catalog.ServicePackages.Models;
using BuildingBlocks.ApiClients.Clients.Catalog.Services.Models;
using BuildingBlocks.ApiClients.Clients.Catalog.Stores.Models;
using BuildingBlocks.ApiClients.Clients.Catalog.UserStore.Models;
using BuildingBlocks.Core.Response;

namespace BuildingBlocks.ApiClients.Clients.Catalog;

public interface ICatalogClient
{
    [Refit.Get("/api/v1/stores/ids")]
    Task<ApiResponse<IEnumerable<StoreDto>>> GetStoreByIdsAsync(string ids, CancellationToken cancellationToken = default);

    [Refit.Get("/api/v1/stores/{id}")]
    Task<ApiResponse<StoreDto>> GetStoreByIdAsync(long id, CancellationToken cancellationToken = default);

    [Refit.Get("/api/v1/userstore/{userId}")]
    Task<ApiResponse<IEnumerable<UserStoreDto>>> GetUserStoreByUserIdAsync(string UserId, CancellationToken cancellationToken = default);

    [Refit.Get("/api/v1/servicepackages/ids")]
    Task<ApiResponse<IEnumerable<ServicePackageDto>>> GetServicePackageIdsAsync(string ids, CancellationToken cancellationToken = default);

    [Refit.Get("/api/v1/services/ids")]
    Task<ApiResponse<IEnumerable<ServiceDto>>> GetServiceIdsAsync(string ids, CancellationToken cancellationToken = default);

    [Refit.Get("/api/v1/services/{id}")]
    Task<ApiResponse<ServiceDto>> GetServiceIdAsync(int id, CancellationToken cancellationToken = default);
}


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
}


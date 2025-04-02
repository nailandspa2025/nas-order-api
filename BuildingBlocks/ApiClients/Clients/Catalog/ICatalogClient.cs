using BuildingBlocks.ApiClients.Clients.Catalog.Stores.Models;
using BuildingBlocks.Core.Response;

namespace BuildingBlocks.ApiClients.Clients.Catalog;

public interface ICatalogClient
{
    [Refit.Get("/api/v1/stores/ids")]
    Task<ApiResponse<IEnumerable<StoreDto>>> GetStoreByIdsAsync(string ids, CancellationToken cancellationToken = default);
}


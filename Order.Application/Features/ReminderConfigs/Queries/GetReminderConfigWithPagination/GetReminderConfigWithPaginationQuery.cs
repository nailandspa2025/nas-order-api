using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.ApiClients.Clients.Catalog;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.ReminderConfigs.Models;

namespace Order.Application.Features.ReminderConfigs.Queries.GetReminderConfigWithPagination;

public record GetReminderConfigWithPaginationQuery : IRequest<ApiResponse<PaginatedList<ReminderConfigDto>>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SearchText { get; set; }
    public bool? IsActive { get; set; }
    public int? StoreId { get; set; }

}

public class GetReminderConfigWithPaginationQueryHandler : IRequestHandler<GetReminderConfigWithPaginationQuery, ApiResponse<PaginatedList<ReminderConfigDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICatalogClient _catalogClient;

    public GetReminderConfigWithPaginationQueryHandler(IOrderDbContext context, IMapper mapper, ICatalogClient catalogClient)
    {
        _context = context;
        _mapper = mapper;
        _catalogClient = catalogClient;
    }

    public async Task<ApiResponse<PaginatedList<ReminderConfigDto>>> Handle(GetReminderConfigWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.ReminderConfig.Where(x => !x.IsDeleted).AsNoTracking();
        if (!request.SearchText.IsNullOrEmpty())
        {
            var lowerSearch = request.SearchText.ToLower();
            query = query.Where(s => s.Name.ToLower().Contains(lowerSearch));
        }
        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }
        if (request.StoreId.HasValue)
        {
            query = query.Where(x => x.StoreId == request.StoreId.Value);
        }

        var paginationResult = await query
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Created)
            .ProjectTo<ReminderConfigDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        var storeIds = paginationResult.Items
            .Select(x => x.StoreId)
            .Distinct()
            .ToList();

        var ids = string.Join(",", storeIds);
        var storeResponse = await _catalogClient.GetStoreByIdsAsync(ids, cancellationToken);
        if (storeResponse.Succeeded && storeResponse.Data != null)
        {
            var storeDict = storeResponse.Data
                .ToDictionary(x => x.Id, x => x.StoreName);

            foreach (var item in paginationResult.Items)
            {
                if (storeDict.TryGetValue(item.StoreId, out var storeName))
                {
                    item.StoreName = storeName;
                }
            }
        }
        return ApiResponse<PaginatedList<ReminderConfigDto>>.Success(paginationResult);
        
    }
}
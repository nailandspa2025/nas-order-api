using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.ApiClients.Clients.Catalog;
using BuildingBlocks.ApiClients.Clients.Catalog.Stores.Models;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Notifications.Models;
using Order.Domain.Enums;

namespace Order.Application.Features.Notifications.Queries.GetNotificationsWithPagination;

public record GetNotificationsForMeWithPaginationQuery: IRequest<ApiResponse<PaginatedList<NotificationDto>>>
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string? SearchText { get; init; }

    public NotificationType? Type { get; init; }
}

public class GetNotificationsForMeWithPaginationQueryHandler : IRequestHandler<GetNotificationsForMeWithPaginationQuery, ApiResponse<PaginatedList<NotificationDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    private readonly ICatalogClient _catalogClient;

    public GetNotificationsForMeWithPaginationQueryHandler(IOrderDbContext context, IMapper mapper, ICurrentUser currentUser, ICatalogClient catalogClient  )
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
        _catalogClient = catalogClient;
    }

    public async Task<ApiResponse<PaginatedList<NotificationDto>>> Handle(GetNotificationsForMeWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = request.SearchText ?? string.Empty;
        var query = _context.Notification.Where(s => s.AccountId == _currentUser.UserId).AsNoTracking();
        if (!paramSearchText.IsNullOrEmpty())
        {
            var lowerSearch = request.SearchText.ToLower();

            query = query.Where(s => s.Content.ToString().ToLower().Contains(lowerSearch));
        }
        
        if (request.Type.HasValue)
        {
            query = query.Where(x => x.Type == request.Type);
        }
        var paginationResult = await query
            .Include( x => x.Booking)
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Created)
            .ProjectTo<NotificationDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);
        try
        {
            {
                var storeIds = paginationResult.Items
                    .Where(x => x.Booking != null)
                    .Select(x => x.Booking!.StoreId)
                    .Distinct()
                    .ToList();

                if (storeIds.Any())
                {
                    var storeResponse = await _catalogClient.GetStoreByIdsAsync(string.Join(",", storeIds), cancellationToken);
                    var stores = storeResponse?.Data ?? new List<StoreDto>();
                    var storeDict = stores.ToDictionary(s => s.Id, s => s);

                    foreach (var item in paginationResult.Items)
                    {
                        if (item.Booking != null && storeDict.TryGetValue((long)item.Booking.StoreId, out var store))
                        {
                            item.Store = store;
                        }
                    }
                }
            }
        }
        catch (Exception) { }

        return ApiResponse<PaginatedList<NotificationDto>>.Success(paginationResult);
    }
}
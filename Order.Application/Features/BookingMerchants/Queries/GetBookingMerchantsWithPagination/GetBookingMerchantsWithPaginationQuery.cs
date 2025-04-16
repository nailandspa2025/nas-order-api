using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.ApiClients.Clients.Catalog;
using BuildingBlocks.ApiClients.Clients.Catalog.Stores.Models;
using BuildingBlocks.ApiClients.Clients.Identity;
using BuildingBlocks.ApiClients.Clients.Identity.Technicians.Models;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Bookings.Models;
using Order.Domain.Enums;

namespace Order.Application.Features.BookingMerchants.Queries.GetBookingMerchantsWithPagination;

public record GetBookingMerchantsWithPaginationQuery: IRequest<ApiResponse<PaginatedList<BookingDto>>>
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string? SearchText { get; init; }

    public BookingStatus? Status { get; init; }
}

public class GetBookingMerchantsWithPaginationQueryHandler : IRequestHandler<GetBookingMerchantsWithPaginationQuery, ApiResponse<PaginatedList<BookingDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityClient _identityClient;
    private readonly ICatalogClient _catalogClient;
    private readonly ICurrentUser _currentUser;

    public GetBookingMerchantsWithPaginationQueryHandler(IOrderDbContext context, IMapper mapper, IIdentityClient identityClient, ICatalogClient catalogClient, ICurrentUser currentUser)
    {
        _context = context;
        _mapper = mapper;
        _identityClient = identityClient;
        _catalogClient = catalogClient;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<PaginatedList<BookingDto>>> Handle(GetBookingMerchantsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = request.SearchText ?? string.Empty;
        var query = _context.Booking.Where(x => !x.IsDeleted).AsNoTracking();
        if (!paramSearchText.IsNullOrEmpty())
        {
            var lowerSearch = request.SearchText.ToLower();

            query = query.Where(s => s.FullName.ToLower().Contains(lowerSearch)
            || s.Phone.Contains(lowerSearch)
            || s.Email.ToLower().Contains(lowerSearch));
        }
        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status);
        }
        List<long> storeIds = new List<long>();
        try
        {
            var response = (await _catalogClient.GetUserStoreByUserIdAsync(_currentUser.UserId))?.Data;
            if (response != null)
            {
                storeIds = response.Select(u => u.StoreId).Distinct().ToList();
            }
        }
        catch (Exception ex) { }
        if (storeIds.Any())
        {
            query = query.Where(x => storeIds.Contains((long)x.StoreId));
        }
        else
        {
            return ApiResponse<PaginatedList<BookingDto>>.Success(
                new PaginatedList<BookingDto>(new List<BookingDto>(), 0, request.PageNumber, request.PageSize));
        }
        var paginationResult = await query
            .OrderBy(x => x.Created)
            .ProjectTo<BookingDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        try
        {
            if (storeIds.Any())
            {
                var stores = (await _catalogClient.GetStoreByIdsAsync(string.Join(",", storeIds), cancellationToken))?.Data;
                var storeDictionary = stores?.ToDictionary(s => s.Id, s => s) ?? new Dictionary<long, StoreDto>();

                foreach (var booking in paginationResult.Items)
                {
                    if (storeDictionary.TryGetValue((long)booking.StoreId, out var store))
                    {
                        booking.Store = store;
                    }
                }
            }
        }
        catch (Exception) { }
        try
        {
            var technicianIds = paginationResult.Items.Select(s => s.TechnicianId).Distinct().ToList();
            if (technicianIds.Any())
            {
                var technicians = (await _identityClient.GetTechnicianByIdsAsync(string.Join(",", technicianIds), cancellationToken))?.Data;
                var technicianDictionary = technicians?.ToDictionary(t => t.Id, t => t) ?? new Dictionary<long, TechnicianDto>();

                foreach (var booking in paginationResult.Items)
                {
                    if (technicianDictionary.TryGetValue((long)booking.TechnicianId, out var technician))
                    {
                        booking.Technician = technician;
                    }
                }
            }
        }
        catch (Exception) { }
        return ApiResponse<PaginatedList<BookingDto>>.Success(paginationResult);
    }
}

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

namespace Order.Application.Features.Bookings.Queries.GetBookings;

public record GetBookingsMeQuery : IRequest<ApiResponse<PaginatedList<BookingDto>>>
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string? SearchText { get; init; }

    public List<string> Status { get; init; } = new List<string>();
}

public class GetBookingsMeQueryHandler : IRequestHandler<GetBookingsMeQuery, ApiResponse<PaginatedList<BookingDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    private readonly IIdentityClient _identityClient;
    private readonly ICatalogClient _catalogClient;

    public GetBookingsMeQueryHandler(IOrderDbContext context, IMapper mapper, ICurrentUser currentUser, IIdentityClient identityClient, ICatalogClient catalogClient)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
        _identityClient = identityClient;
        _catalogClient = catalogClient;
    }

    public async Task<ApiResponse<PaginatedList<BookingDto>>> Handle(GetBookingsMeQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = request.SearchText ?? string.Empty;
        var query = _context.Booking.AsNoTracking();
        if (!paramSearchText.IsNullOrEmpty())
        {
            var lowerSearch = request.SearchText.ToLower();

            query = query.Where(s => s.FullName.ToLower().Contains(lowerSearch)
            || s.Phone.Contains(lowerSearch)
            || s.Email.ToLower().Contains(lowerSearch));
        }
        if (request.Status != null && request.Status.Count > 0)
        {
            query = query.Where(x => request.Status.Contains(x.Status.ToString()));
        }
        var paginationResult = await query
            .Where(x => !x.IsDeleted && x.UserId == _currentUser.UserId )
            .OrderBy(x => x.Created)
            .ProjectTo<BookingDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        try
        {
            var storeIds = paginationResult.Items.Select(x => x.StoreId).Distinct().ToList();
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


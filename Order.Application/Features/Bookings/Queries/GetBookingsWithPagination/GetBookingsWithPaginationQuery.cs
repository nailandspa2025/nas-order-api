using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.ApiClients.Clients.Catalog;
using BuildingBlocks.ApiClients.Clients.Catalog.Services.Models;
using BuildingBlocks.ApiClients.Clients.Catalog.Stores.Models;
using BuildingBlocks.ApiClients.Clients.Identity;
using BuildingBlocks.ApiClients.Clients.Identity.Technicians.Models;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Bookings.Models;
using Order.Domain.Enums;

namespace Order.Application.Features.Bookings.Queries.GetBookingsWithPagination;

public record GetBookingsWithPaginationQuery : IRequest<ApiResponse<PaginatedList<BookingDto>>>
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string? SearchText { get; init; }

    public BookingStatus ? Status { get; init; }
}

public class GetBookingsWithPaginationQueryHandler : IRequestHandler<GetBookingsWithPaginationQuery, ApiResponse<PaginatedList<BookingDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityClient _identityClient;
    private readonly ICatalogClient _catalogClient;

    public GetBookingsWithPaginationQueryHandler (IOrderDbContext context, IMapper mapper, IIdentityClient identityClient, ICatalogClient catalogClient)
    {
        _context = context;
        _mapper = mapper;
        _identityClient = identityClient;
        _catalogClient = catalogClient;
    }

    public async Task<ApiResponse<PaginatedList<BookingDto>>> Handle(GetBookingsWithPaginationQuery request, CancellationToken cancellationToken)
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
        var paginationResult = await query
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
        try
        {
            var serviceIds = paginationResult.Items.Select(s => s.ServiceId).Distinct().ToList();
            if (serviceIds.Any())
            {
                var services = (await _catalogClient.GetServiceIdsAsync(string.Join(",", serviceIds), cancellationToken))?.Data;
                var serviceDictionary = services?.ToDictionary(p => p.Id, p => p) ?? new Dictionary<int, ServiceDto>();
                foreach (var item in paginationResult.Items)
                {
                    if (serviceDictionary.TryGetValue((int)item.ServiceId, out var service))
                    {
                        item.Service = service;
                    }
                }
            }
        }
        catch (Exception) { }
        return ApiResponse<PaginatedList<BookingDto>>.Success(paginationResult);
    }
}

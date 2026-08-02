using AutoMapper;
using BuildingBlocks.ApiClients.Clients.Catalog;
using BuildingBlocks.ApiClients.Clients.Catalog.Services.Models;
using BuildingBlocks.ApiClients.Clients.Catalog.Stores.Models;
using BuildingBlocks.ApiClients.Clients.Identity;
using BuildingBlocks.ApiClients.Clients.Identity.Technicians.Models;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Bookings.Models;
using Order.Domain.Enums;

namespace Order.Application.Features.Bookings.Queries.GetWalkinBookings;

public record GetPendingWalkInBookingsByPhoneQuery : IRequest<ApiResponse<IEnumerable<BookingDto>>>
{
    public string Phone { get; init; } = default!;
}

public class GetPendingWalkInBookingsByPhoneQueryHandler : IRequestHandler<GetPendingWalkInBookingsByPhoneQuery, ApiResponse<IEnumerable<BookingDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    private readonly ICatalogClient _catalogClient;
    private readonly IIdentityClient _identityClient;

    private readonly ILogger<GetPendingWalkInBookingsByPhoneQueryHandler> _logger;
    public GetPendingWalkInBookingsByPhoneQueryHandler(IOrderDbContext context, IMapper mapper, ICurrentUser currentUser, ICatalogClient catalogClient, ILogger<GetPendingWalkInBookingsByPhoneQueryHandler> logger, IIdentityClient identityClient)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
        _catalogClient = catalogClient;
        _logger = logger;
        _identityClient = identityClient;
    }
    public async Task<ApiResponse<IEnumerable<BookingDto>>> Handle(GetPendingWalkInBookingsByPhoneQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.Now.Date;
        var bookings = await _context.Booking
            .Include(x => x.BookingTechnicians)
            .ThenInclude(x =>x.Services)
            .Include(x => x.BookingSnaps)
            .Include(x => x.BookingSnapGroups)
            .Where(b => b.Phone == request.Phone
                && b.Status == BookingStatus.Pending
                && b.BookingDate == today
                && b.StoreId == Convert.ToInt64(_currentUser.StoreId)
            ).ToListAsync(cancellationToken);
        var bookingDtos = _mapper.Map<List<BookingDto>>(bookings);
        try
        {
            var storeIds = bookingDtos
                .Select(x => x.StoreId)
                .Distinct()
                .ToList();

            if (storeIds.Any())
            {
                var stores = (await _catalogClient
                    .GetStoreByIdsAsync(string.Join(",", storeIds), cancellationToken))
                    ?.Data;

                var storeDictionary = stores?
                    .ToDictionary(x => x.Id, x => x)
                    ?? new Dictionary<long, StoreDto>();

                foreach (var booking in bookingDtos)
                {
                    if (booking.StoreId.HasValue &&
                        storeDictionary.TryGetValue(booking.StoreId.Value, out var store))
                    {
                        booking.Store = store;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pending walk-in bookings by phone");
        }
        Dictionary<long, TechnicianDto> technicianDictionary = new();
        Dictionary<int, ServiceDto> serviceDictionary = new();
        try
        {
            var technicianIds = bookingDtos.SelectMany(b => b.TechnicianIds).Distinct().ToList();
            if (technicianIds.Any())
            {
                var technicians = (await _identityClient
                    .GetTechnicianByIdsAsync(string.Join(",", technicianIds), cancellationToken))
                    ?.Data;

                technicianDictionary = technicians?
                    .ToDictionary(t => t.Id, t => t)
                    ?? new Dictionary<long, TechnicianDto>();
            }
        }
        catch (Exception ex) { }
        try
        {
            var serviceIds = bookingDtos.SelectMany(b => b.ServiceIds).Distinct().ToList();
            if (serviceIds.Any())
            {
                var services = (await _catalogClient
                    .GetServiceIdsAsync(string.Join(",", serviceIds), cancellationToken))
                    ?.Data;

                serviceDictionary = services?
                    .ToDictionary(s => s.Id, s => s)
                    ?? new Dictionary<int, ServiceDto>();
            }
        }
        catch (Exception ex) { }
        foreach (var (entity, dto) in bookings.Zip(bookingDtos))
        {
            dto.Technicians = entity.BookingTechnicians
                .Select(bt => new BookingTechnicianDto
                {
                    TechnicianId = bt.TechnicianId,
                    Technician = technicianDictionary.GetValueOrDefault(bt.TechnicianId),
                    Services = bt.Services
                        .Where(s => serviceDictionary.ContainsKey(s.ServiceId))
                        .Select(s => serviceDictionary[s.ServiceId])
                        .ToList()
                })
                .ToList();
        }
        return ApiResponse<IEnumerable<BookingDto>>.Success(bookingDtos);
    }
}
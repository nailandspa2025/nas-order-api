using AutoMapper;
using BuildingBlocks.ApiClients.Clients.Catalog;
using BuildingBlocks.ApiClients.Clients.Catalog.Services.Models;
using BuildingBlocks.ApiClients.Clients.Identity;
using BuildingBlocks.ApiClients.Clients.Identity.Technicians.Models;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Commissions.Models;
using Order.Domain.Enums;

namespace Order.Application.Features.Commissions.Queries.GetCommissionsWithPagination;

public record GetCommissionForMerchantWithPaginationQuery : IRequest<ApiResponse<PaginatedList<CommissionDetailDto>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public DateTime? FromDate { get; init; }
    public DateTime? EndDate { get; init; }
}

public class GetCommissionForMerchantWithPaginationQueryHandler : IRequestHandler<GetCommissionForMerchantWithPaginationQuery, ApiResponse<PaginatedList<CommissionDetailDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityClient _identityClient;
    private readonly ICatalogClient _catalogClient;
    private readonly ICurrentUser _currentUser;
    public GetCommissionForMerchantWithPaginationQueryHandler(IOrderDbContext context, IMapper mapper, IIdentityClient identityClient, ICatalogClient catalogClient, ICurrentUser currentUser)
    {
        _context = context;
        _mapper = mapper;
        _identityClient = identityClient;
        _catalogClient = catalogClient;
        _currentUser = currentUser;
    }
    public async Task<ApiResponse<PaginatedList<CommissionDetailDto>>> Handle(GetCommissionForMerchantWithPaginationQuery request, CancellationToken cancellationToken)
    {
        // 1. Build query với filter trước
        var bookingQuery = _context.Booking
            .Where(x => !x.IsDeleted && x.Status == BookingStatus.Completed)
            .AsNoTracking();

        // Apply date filters
        if (request.FromDate.HasValue)
        {
            var from = request.FromDate.Value.Date;
            bookingQuery = bookingQuery.Where(x => x.BookingDate >= from);
        }

        if (request.EndDate.HasValue)
        {
            var end = request.EndDate.Value.Date.AddDays(1);
            bookingQuery = bookingQuery.Where(x => x.BookingDate < end);
        }

        // Get user permissions
        List<long> storeIds = new List<long>();
        bool isOwner = false;
        long? technicianId = null;

        try
        {
            var response = (await _identityClient.GetUserMerchantByIdAsync(_currentUser.UserId))?.Data;
            if (response != null)
            {
                isOwner = response.IsOwner;
                if (isOwner && response.StoreIds?.Any() == true)
                {
                    storeIds = response.StoreIds.Distinct().ToList();
                }
                else if (!isOwner)
                {
                    technicianId = response.TechnicianId;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"=== Error getting user info: {ex.Message}");
        }

        // ✅ Apply permission filters BEFORE SelectMany
        if (isOwner)
        {
            if (!storeIds.Any())
            {
                return ApiResponse<PaginatedList<CommissionDetailDto>>.Success(
                    new PaginatedList<CommissionDetailDto>(new List<CommissionDetailDto>(), 0, request.PageNumber, request.PageSize));
            }
            bookingQuery = bookingQuery.Where(x => storeIds.Contains(x.StoreId ?? 0));
        }
        else
        {
            if (!technicianId.HasValue)
            {
                return ApiResponse<PaginatedList<CommissionDetailDto>>.Success(
                    new PaginatedList<CommissionDetailDto>(new List<CommissionDetailDto>(), 0, request.PageNumber, request.PageSize));
            }
            // ✅ Filter by technician BEFORE SelectMany
            bookingQuery = bookingQuery.Where(x => x.BookingTechnicians.Any(bt => bt.TechnicianId == technicianId));
        }

        // 2. Now do SelectMany to flatten
        var query = bookingQuery
            .SelectMany(booking => booking.BookingTechnicians
                .SelectMany(technician => technician.Services
                    .Select(service => new CommissionDetailDto
                    {
                        BookingId = booking.Id,
                        StoreId = booking.StoreId ?? 0,  // ✅ Add StoreId
                        BookingDate = booking.BookingDate,
                        BookingTime = booking.BookingTime,
                        Status = booking.Status,
                        ServiceId = service.ServiceId,
                        TechnicianId = technician.TechnicianId,
                        ServiceName = string.Empty,
                        TechnicianName = string.Empty
                    })
                )
            );

        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);

        // Get paginated results
        var items = await query
            .OrderByDescending(x => x.BookingDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // Enrich with service and technician names
        if (items.Any())
        {
            var allServiceIds = items.Select(x => x.ServiceId).Distinct().ToList();
            var allTechnicianIds = items.Select(x => x.TechnicianId).Distinct().ToList();

            // Fetch service details
            Dictionary<int, ServiceDto> serviceDictionary = new();
            if (allServiceIds.Any())
            {
                try
                {
                    var services = (await _catalogClient
                        .GetServiceIdsAsync(string.Join(",", allServiceIds), cancellationToken))
                        ?.Data;

                    serviceDictionary = services?
                        .ToDictionary(s => s.Id, s => s)
                        ?? new Dictionary<int, ServiceDto>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"=== Error fetching services: {ex.Message}");
                }
            }

            // Fetch technician details
            Dictionary<long, TechnicianDto> technicianDictionary = new();
            if (allTechnicianIds.Any())
            {
                try
                {
                    var idsString = string.Join(",", allTechnicianIds);
                    Console.WriteLine($"=== Calling API with IDs: {idsString}");

                    var response = await _identityClient
                        .GetTechnicianByIdsAsync(idsString, cancellationToken);

                    var technicians = response?.Data;
                    if (technicians != null && technicians.Any())
                    {
                        foreach (var tech in technicians)
                        {
                            Console.WriteLine($"=== Technician: Id={tech.Id}, Name={tech.TechnicianName}");
                        }
                        technicianDictionary = technicians.ToDictionary(t => t.Id, t => t);
                    }
                    else
                    {
                        Console.WriteLine("=== No technicians returned from API");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"=== Error fetching technicians: {ex.Message}");
                    Console.WriteLine($"=== StackTrace: {ex.StackTrace}");
                }
            }

            // Enrich data
            foreach (var item in items)
            {
                if (serviceDictionary.TryGetValue(item.ServiceId, out var service))
                {
                    item.ServiceName = service.Name ?? "Unknown Service";
                }

                if (technicianDictionary.TryGetValue(item.TechnicianId, out var tech))
                {
                    item.TechnicianName = tech.TechnicianName ?? "Unknown";
                }
            }
        }

        var paginationResult = new PaginatedList<CommissionDetailDto>(
            items,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return ApiResponse<PaginatedList<CommissionDetailDto>>.Success(paginationResult);
    }
}
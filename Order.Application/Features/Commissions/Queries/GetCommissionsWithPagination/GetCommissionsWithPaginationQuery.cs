using AutoMapper;
using BuildingBlocks.ApiClients.Clients.Catalog;
using BuildingBlocks.ApiClients.Clients.Catalog.Services.Models;
using BuildingBlocks.ApiClients.Clients.Identity;
using BuildingBlocks.ApiClients.Clients.Identity.Technicians.Models;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Commissions.Models;
using Order.Domain.Enums;

namespace Order.Application.Features.Commissions.Queries.GetCommissionsWithPagination;

public record GetCommissionsWithPaginationQuery : IRequest<ApiResponse<PaginatedList<CommissionDetailDto>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public DateTime? FromDate { get; init; }
    public DateTime? EndDate { get; init; }
    public long? TechnicianId { get; init; }  // Nullable
    public int? ServiceId { get; init; } 
    public long ? StoreId { get; init; }

}

public class GetCommissionsWithPaginationQueryHandler : IRequestHandler<GetCommissionsWithPaginationQuery, ApiResponse<PaginatedList<CommissionDetailDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityClient _identityClient;
    private readonly ICatalogClient _catalogClient;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<GetCommissionsWithPaginationQueryHandler> _logger;
    public GetCommissionsWithPaginationQueryHandler(IOrderDbContext context, IMapper mapper, IIdentityClient identityClient, ICatalogClient catalogClient, ICurrentUser currentUser, ILogger<GetCommissionsWithPaginationQueryHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _identityClient = identityClient;
        _catalogClient = catalogClient;
        _currentUser = currentUser;
        _logger = logger;
    }
    public async Task<ApiResponse<PaginatedList<CommissionDetailDto>>> Handle(GetCommissionsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Booking
        .Where(x => !x.IsDeleted && (x.Status == BookingStatus.Completed || x.Status == BookingStatus.Close))
        .SelectMany(booking => booking.BookingTechnicians
            .SelectMany(technician => technician.Services
                .Select(service => new CommissionDetailDto
                {
                    BookingId = booking.Id,
                    StoreId = booking.StoreId ?? 0, 
                    BookingDate = booking.BookingDate,
                    BookingTime = booking.BookingTime,
                    Status = booking.Status,
                    ServiceId = service.ServiceId,
                    CommissionAmount= 0,
                    TechnicianId = technician.TechnicianId,
                    ServiceName = string.Empty,
                    TechnicianName = string.Empty
                })
            )
        )
        .AsNoTracking();
        if (request.FromDate.HasValue)
        {
            var from = request.FromDate.Value.Date;
            query = query.Where(x => x.BookingDate >= from);
        }

        if (request.EndDate.HasValue)
        {
            var end = request.EndDate.Value.Date;
            query = query.Where(x => x.BookingDate < end);
        }
        if (request.TechnicianId.HasValue)
        {
            query = query.Where(x => x.TechnicianId == request.TechnicianId.Value);
        }
        if (request.ServiceId.HasValue)
        {
            query = query.Where(x => x.ServiceId == request.ServiceId.Value);
        }
        if (request.StoreId.HasValue)
        {
            query = query.Where(x => x.StoreId == request.StoreId.Value);
        }
        var storeIds = await query
            .Select(x => x.StoreId)
            .Distinct()
            .ToListAsync(cancellationToken);
        var commissionStoreIds = new List<long>();

        if (storeIds.Any())
        {
            try
            {
                var storesResponse = await _catalogClient
                .GetStoreByIdsAsync(
                    string.Join(",", storeIds),
                    cancellationToken);

                commissionStoreIds = storesResponse?.Data?
                    .Where(x => x.IsCommission)
                    .Select(x => x.Id)
                    .ToList()
                    ?? new List<long>();
            }
            catch (Exception ex)
            {
                 _logger.LogError(
                    ex,
                    "Failed to get Store information for StoreIds: {StoreIds}",
                    string.Join(",", storeIds));
            }
        }
        if (!commissionStoreIds.Any())
        {
            query = query.Where(x => false);
        }
        else
        {
            query = query.Where(x =>
                commissionStoreIds.Contains(x.StoreId));
        }
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.BookingDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

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
                    // Log error
                    Console.WriteLine($"=== API Response Error: {ex.Message}");
                }
            }

            // Fetch technician details
            Dictionary<long, TechnicianDto> technicianDictionary = new();
            if (allTechnicianIds.Any())
            {
                try
                {
                    var technicians = (await _identityClient
                        .GetTechnicianByIdsAsync(string.Join(",", allTechnicianIds), cancellationToken))
                        ?.Data;

                    technicianDictionary = technicians?
                        .ToDictionary(t => t.Id, t => t)
                        ?? new Dictionary<long, TechnicianDto>();
                    Console.WriteLine($"=== API Response Success: {technicianDictionary}");
                }
                catch (Exception ex)
                {
                    // Log error
                    Console.WriteLine($"=== API Response Error: {ex.Message}");
                }
            }
            foreach (var item in items)
            {
                if (serviceDictionary.TryGetValue(item.ServiceId, out var service))
                {
                    item.ServiceName = service.Name ?? "Unknown Service";
                    if (service.CommissionType == 1)
                    {
                        item.CommissionAmount = service.Commission;
                    }
                    else if (service.CommissionType == 2)
                    {
                        item.CommissionAmount = service.PriceFrom * service.Commission / 100;
                    }
                    else
                    {
                         item.CommissionAmount = 0;
                    }
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
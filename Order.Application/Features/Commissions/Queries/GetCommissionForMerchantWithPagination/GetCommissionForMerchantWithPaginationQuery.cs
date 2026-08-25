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

public record GetCommissionForMerchantWithPaginationQuery : IRequest<ApiResponse<PaginatedList<CommissionDetailDto>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public DateTime? FromDate { get; init; }
    public DateTime? EndDate { get; init; }
    public long? TechnicianId { get; init; }
    public int? ServiceId { get; init; }
}

public class GetCommissionForMerchantWithPaginationQueryHandler : IRequestHandler<GetCommissionForMerchantWithPaginationQuery, ApiResponse<PaginatedList<CommissionDetailDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityClient _identityClient;
    private readonly ICatalogClient _catalogClient;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<GetCommissionForMerchantWithPaginationQueryHandler> _logger;

    public GetCommissionForMerchantWithPaginationQueryHandler(
        IOrderDbContext context,
        IMapper mapper,
        IIdentityClient identityClient,
        ICatalogClient catalogClient,
        ICurrentUser currentUser,
        ILogger<GetCommissionForMerchantWithPaginationQueryHandler> logger)
    {
        _context = context;
        _mapper = mapper;
        _identityClient = identityClient;
        _catalogClient = catalogClient;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ApiResponse<PaginatedList<CommissionDetailDto>>> Handle(GetCommissionForMerchantWithPaginationQuery request, CancellationToken cancellationToken)
    {
        // 1. Query cơ sở: lấy các booking đã hoàn thành, chưa xóa
        var bookingQuery = _context.Booking
            .Where(x => !x.IsDeleted && (x.Status == BookingStatus.Completed || x.Status == BookingStatus.Close))
            .AsNoTracking();

        // 2. Lọc theo ngày (nếu có)
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

        // 3. Lấy quyền của user
        List<long> storeIds = new();
        bool isOwner = false;
        long? currentTechnicianId = null;

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
                    currentTechnicianId = response.TechnicianId;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"=== Error getting user info: {ex.Message}");
        }

        // 4. Áp dụng filter quyền
        if (isOwner)
        {
            if (!storeIds.Any())
                return ApiResponse<PaginatedList<CommissionDetailDto>>.Success(
                    new PaginatedList<CommissionDetailDto>(new List<CommissionDetailDto>(), 0, request.PageNumber, request.PageSize));

            bookingQuery = bookingQuery.Where(x => storeIds.Contains(x.StoreId ?? 0));
        }
        else
        {
            if (!currentTechnicianId.HasValue)
                return ApiResponse<PaginatedList<CommissionDetailDto>>.Success(
                    new PaginatedList<CommissionDetailDto>(new List<CommissionDetailDto>(), 0, request.PageNumber, request.PageSize));

            // Nếu là technician, chỉ lấy booking có chứa technician đó
            bookingQuery = bookingQuery.Where(x => x.BookingTechnicians.Any(bt => bt.TechnicianId == currentTechnicianId.Value));
        }

        // 5. Áp dụng filter TechnicianId và ServiceId từ request (lọc trên booking)
        if (request.TechnicianId.HasValue)
        {
            bookingQuery = bookingQuery.Where(b => b.BookingTechnicians.Any(bt => bt.TechnicianId == request.TechnicianId.Value));
        }
        if (request.ServiceId.HasValue)
        {
            bookingQuery = bookingQuery.Where(b => b.BookingTechnicians.Any(bt => bt.Services.Any(s => s.ServiceId == request.ServiceId.Value)));
        }
        var bookingStoreIds = await bookingQuery
                .Select(x => x.StoreId!.Value)
                .Distinct()
                .ToListAsync(cancellationToken);
        var commissionStoreIds = new List<long>();
        if (bookingStoreIds.Any())
        {
            try
            {
                var storesResponse =
                    await _catalogClient.GetStoreByIdsAsync(
                        string.Join(",", bookingStoreIds),
                        cancellationToken);

                commissionStoreIds =
                    storesResponse?.Data?
                        .Where(x => x.IsCommission)
                        .Select(x => x.Id)
                        .Distinct()
                        .ToList()
                    ?? new List<long>();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to get Store information. StoreIds: {StoreIds}",
                    string.Join(",", bookingStoreIds));

                commissionStoreIds.Clear();
            }
        }
        if (!commissionStoreIds.Any())
        {
            return ApiResponse<
                PaginatedList<CommissionDetailDto>>
                .Success(
                    new PaginatedList<CommissionDetailDto>(
                        new List<CommissionDetailDto>(),
                        0,
                        request.PageNumber,
                        request.PageSize));
        }
        bookingQuery = bookingQuery.Where(x =>
        commissionStoreIds.Contains(x.StoreId!.Value));
        // 6. SelectMany để làm phẳng dữ liệu, nhưng chỉ lấy đúng technician/service theo request
        var query = bookingQuery
            .SelectMany(booking => booking.BookingTechnicians
                // Lọc technician: nếu có request.TechnicianId thì dùng, còn không thì lấy tất cả, nhưng vẫn bảo toàn quyền
                .Where(bt => (!request.TechnicianId.HasValue || bt.TechnicianId == request.TechnicianId.Value)
                             && (!currentTechnicianId.HasValue || bt.TechnicianId == currentTechnicianId.Value))
                .SelectMany(technician => technician.Services
                    // Lọc service theo request.ServiceId (nếu có)
                    .Where(s => !request.ServiceId.HasValue || s.ServiceId == request.ServiceId.Value)
                    .Select(service => new CommissionDetailDto
                    {
                        BookingId = booking.Id,
                        StoreId = booking.StoreId ?? 0,
                        BookingDate = booking.BookingDate,
                        BookingTime = booking.BookingTime,
                        Status = booking.Status,
                        ServiceId = service.ServiceId,
                        TechnicianId = technician.TechnicianId,
                        CommissionAmount = 0,        // sẽ được gán sau
                        ServiceName = string.Empty,   // sẽ được gán sau
                        TechnicianName = string.Empty // sẽ được gán sau
                    })
                )
            );

        

        // 7. Đếm tổng số và phân trang
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.BookingDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        // 8. Enrich thông tin (tên service, tên technician, commission)
        if (items.Any())
        {
            var allServiceIds = items.Select(x => x.ServiceId).Distinct().ToList();
            var allTechnicianIds = items.Select(x => x.TechnicianId).Distinct().ToList();

            // 8a. Lấy service từ Catalog API
            Dictionary<int, ServiceDto> serviceDict = new();
            if (allServiceIds.Any())
            {
                try
                {
                    var services = (await _catalogClient
                        .GetServiceIdsAsync(string.Join(",", allServiceIds), cancellationToken))
                        ?.Data;
                    if (services != null && services.Any())
                        serviceDict = services.ToDictionary(s => s.Id, s => s);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"=== Error fetching services: {ex.Message}");
                }
            }

            // 8b. Lấy technician từ Identity API
            Dictionary<long, TechnicianDto> technicianDict = new();
            if (allTechnicianIds.Any())
            {
                try
                {
                    var idsString = string.Join(",", allTechnicianIds);
                    var response = await _identityClient
                        .GetTechnicianByIdsAsync(idsString, cancellationToken);
                    var techs = response?.Data;
                    if (techs != null && techs.Any())
                        technicianDict = techs.ToDictionary(t => t.Id, t => t);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"=== Error fetching technicians: {ex.Message}");
                }
            }

            // 8c. Gán dữ liệu
            foreach (var item in items)
            {
                // Tên service và commission
                if (serviceDict.TryGetValue(item.ServiceId, out var svc))
                {
                    item.ServiceName = svc.Name ?? "Unknown Service";
                    if (svc.CommissionType == 1)
                    {
                        item.CommissionAmount = svc.Commission;
                    }
                    else if (svc.CommissionType == 2)
                    {
                        item.CommissionAmount = svc.PriceFrom * svc.Commission / 100;
                    }
                    else
                    {
                        item.CommissionAmount = 0;
                    }
                }
                else
                {
                    item.ServiceName = "Unknown Service";
                    item.CommissionAmount = 0;
                }

                // Tên technician
                if (technicianDict.TryGetValue(item.TechnicianId, out var tech))
                    item.TechnicianName = tech.TechnicianName ?? "Unknown";
                else
                    item.TechnicianName = "Unknown";
            }
        }

        // 9. Trả về kết quả phân trang
        var paginationResult = new PaginatedList<CommissionDetailDto>(
            items,
            totalCount,
            request.PageNumber,
            request.PageSize);

        return ApiResponse<PaginatedList<CommissionDetailDto>>.Success(paginationResult);
    }
}
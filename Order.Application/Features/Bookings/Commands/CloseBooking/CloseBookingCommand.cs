using BuildingBlocks.ApiClients.Clients.Catalog;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Order.Application.Common.Interfaces;
using Order.Domain.Enums;

namespace Order.Application.Features.Bookings.Commands.CloseBooking;

public record CloseBookingCommand : IRequest<Unit>;

public class CloseBookingCommandHandler
    : IRequestHandler<CloseBookingCommand, Unit>
{
    private readonly IOrderDbContext _context;
    private readonly ICatalogClient _catalogClient;
    private readonly ILogger<CloseBookingCommandHandler> _logger;

    public CloseBookingCommandHandler(
        IOrderDbContext context,
        ICatalogClient catalogClient,
        ILogger<CloseBookingCommandHandler> logger)
    {
        _context = context;
        _catalogClient = catalogClient;
        _logger = logger;
    }

    public async Task<Unit> Handle(
        CloseBookingCommand request,
        CancellationToken cancellationToken)
    {
        var utcNow = DateTimeOffset.UtcNow;

        // ============================================================
        // 1. Lấy các Booking chưa ở trạng thái cuối
        // ============================================================

        var bookings = await _context.Booking
            .AsNoTracking()
            .Where(x =>
                x.Status != BookingStatus.Completed &&
                x.Status != BookingStatus.Cancelled &&
                x.Status != BookingStatus.Close)
            .Select(x => new
            {
                x.Id,
                x.StoreId,
                x.Status
            })
            .ToListAsync(cancellationToken);

        if (bookings.Count == 0)
        {
            _logger.LogDebug(
                "Close booking job: no open bookings found.");

            return Unit.Value;
        }

        // ============================================================
        // 2. Lấy danh sách StoreId
        // ============================================================

        var storeIds = bookings
            .Where(x => x.StoreId.HasValue)
            .Select(x => x.StoreId!.Value)
            .Distinct()
            .ToList();

        if (storeIds.Count == 0)
        {
            _logger.LogDebug(
                "Close booking job: no valid StoreId found.");

            return Unit.Value;
        }

        // ============================================================
        // 3. Lấy timezone của các Store
        // ============================================================

        var response = await _catalogClient.GetStoreByIdsAsync(
            string.Join(",", storeIds),
            cancellationToken);

        var stores = response?.Data;

        if (stores == null)
        {
            _logger.LogWarning(
                "Close booking job: no store information found.");

            return Unit.Value;
        }

        var storeMap = stores
            .Where(x => x.Id != null && !string.IsNullOrWhiteSpace(x.TimeZone))
            .GroupBy(x => x.Id)
            .ToDictionary(
                x => x.Key,
                x => x.First().TimeZone);

        // ============================================================
        // 4. Xác định các Store đang ở thời điểm chuyển sang ngày mới
        // ============================================================

        var storeIdsToClose = new HashSet<long>();

        foreach (var storeId in storeIds)
        {
            if (!storeMap.TryGetValue(storeId, out var timeZoneId))
            {
                _logger.LogWarning(
                    "Cannot find timezone for Store {StoreId}",
                    storeId);

                continue;
            }

            TimeZoneInfo timeZone;

            try
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(
                    timeZoneId);
            }
            catch (TimeZoneNotFoundException)
            {
                _logger.LogError(
                    "Timezone {TimeZoneId} not found for Store {StoreId}",
                    timeZoneId,
                    storeId);

                continue;
            }
            catch (InvalidTimeZoneException)
            {
                _logger.LogError(
                    "Invalid timezone {TimeZoneId} for Store {StoreId}",
                    timeZoneId,
                    storeId);

                continue;
            }

            // UTC -> giờ local của Store
            var storeNow = TimeZoneInfo.ConvertTime(
                utcNow,
                timeZone);

            _logger.LogDebug(
                "Store {StoreId}, TimeZone {TimeZoneId}, LocalTime {LocalTime}",
                storeId,
                timeZoneId,
                storeNow);

            // ========================================================
            // Chỉ xử lý trong phút đầu tiên của ngày mới.
            //
            // Ví dụ:
            // Store VN:
            // 2026-08-23 00:00
            //
            // Store Japan:
            // 2026-08-23 00:00
            // ========================================================

            if (storeNow.Hour == 0 &&
                storeNow.Minute == 0)
            {
                storeIdsToClose.Add(storeId);
            }
        }

        if (storeIdsToClose.Count == 0)
        {
            return Unit.Value;
        }

        // ============================================================
        // 5. Lấy Booking thuộc các Store cần Close
        // ============================================================

        var bookingIdsToClose = bookings
            .Where(x =>
                x.StoreId.HasValue &&
                storeIdsToClose.Contains(x.StoreId.Value))
            .Select(x => x.Id)
            .ToList();

        if (bookingIdsToClose.Count == 0)
        {
            return Unit.Value;
        }

        // ============================================================
        // 6. Update trực tiếp Database
        //
        // Có thêm điều kiện Status để tránh:
        //
        // Job đọc Pending
        //        ↓
        // Booking được Completed
        //        ↓
        // Job update
        //
        // Booking Completed sẽ không bị đổi thành Close.
        // ============================================================

        var affectedRows = await _context.Booking
            .Where(x =>
                bookingIdsToClose.Contains(x.Id) &&
                x.Status != BookingStatus.Completed &&
                x.Status != BookingStatus.Cancelled &&
                x.Status != BookingStatus.Close)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(
                        x => x.Status,
                        BookingStatus.Close)
                    .SetProperty(
                        x => x.LastModified,
                        DateTime.UtcNow),
                cancellationToken);

        // ============================================================
        // 7. Log
        // ============================================================

        _logger.LogInformation(
            "Close booking job completed. " +
            "StoresToClose: {StoreCount}, " +
            "BookingsClosed: {BookingCount}",
            storeIdsToClose.Count,
            affectedRows);

        return Unit.Value;
    }
}
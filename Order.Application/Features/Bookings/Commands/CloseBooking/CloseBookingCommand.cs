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

        // 1. Lấy các booking chưa ở trạng thái cuối
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
            return Unit.Value;
        }

        // 2. Lấy danh sách StoreId
        var storeIds = bookings
            .Select(x => x.StoreId)
            .Distinct()
            .ToList();

        // 3. Gọi Catalog API để lấy timezone của các Store
        var stores = (await _catalogClient.GetStoreByIdsAsync(
            string.Join(",", storeIds),
            cancellationToken
        ))?.Data;

        if (stores == null)
        {
            _logger.LogWarning(
                "No store information found for booking close job.");

            return Unit.Value;
        }

        var storeMap = stores
            .ToDictionary(
                x => x.Id,
                x => x.TimeZone);

        // 4. Những Booking cần Close
        var bookingIdsToClose = new List<int>();

        // 5. Group Booking theo Store
        foreach (var storeGroup in bookings.GroupBy(x => x.StoreId))
        {
            var storeId = storeGroup.Key!.Value;

            // Không tìm thấy Store
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

            // 6. Convert UTC → giờ của Store
            var storeNow = TimeZoneInfo.ConvertTime(
                utcNow,
                timeZone);

            _logger.LogDebug(
                "Store {StoreId}, TimeZone {TimeZoneId}, LocalTime {StoreNow}",
                storeId,
                timeZoneId,
                storeNow);

            // 7. Chỉ xử lý lúc 23:59 theo giờ Store
            if (storeNow.Hour != 23 ||
                storeNow.Minute != 59)
            {
                continue;
            }

            // 8. Add booking của Store này vào danh sách cần Close
            bookingIdsToClose.AddRange(
                storeGroup.Select(x => x.Id));
        }

        if (bookingIdsToClose.Count == 0)
        {
            return Unit.Value;
        }

        // 9. Update trực tiếp database
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

        _logger.LogInformation(
            "Close booking job completed. " +
            "Stores: {StoreCount}, Bookings: {BookingCount}",
            storeMap.Count,
            affectedRows);

        return Unit.Value;
    }
}
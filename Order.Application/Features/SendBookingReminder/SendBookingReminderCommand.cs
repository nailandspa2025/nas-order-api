using BuildingBlocks.ApiClients.Clients.AccountDevice.Models;
using BuildingBlocks.ApiClients.Clients.Identity;
using BuildingBlocks.Common.Firebase;
using FirebaseAdmin.Messaging;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Order.Application.Common.Interfaces;
using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Application.Features.SendBookingReminder;

public record SendBookingReminderCommand : IRequest<Unit>;
public class SendBookingReminderCommandHandler : IRequestHandler<SendBookingReminderCommand, Unit>
{
    private readonly IOrderDbContext _context;
    private readonly IIdentityClient _identityClient;
    private readonly IFirebaseService _firebaseService;
    private readonly ILogger<SendBookingReminderCommandHandler> _logger;
    private readonly IPublishEndpoint _publishEndpoint;

    public SendBookingReminderCommandHandler(IOrderDbContext context, IIdentityClient identityClient, IFirebaseService firebaseService, ILogger<SendBookingReminderCommandHandler> logger, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _identityClient = identityClient;
        _firebaseService = firebaseService;
        _logger = logger;
        _publishEndpoint = publishEndpoint;
    }
    public async Task<Unit> Handle(SendBookingReminderCommand request, CancellationToken cancellationToken)
    {
       var now = DateTime.UtcNow.AddHours(7);
        // Load configs
        var configs = await _context.ReminderConfig
            .Where(x => x.IsActive && !x.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var config in configs)
        {
            // 👉 Lấy booking trong khoảng an toàn ±1 ngày
            var fromDate = now.Date.AddDays(-1);
            var toDate = now.Date.AddDays(1);

            var bookings = await _context.Booking
                .Where(x =>
                    x.StoreId == config.StoreId &&
                    x.BookingDate >= fromDate &&
                    x.BookingDate <= toDate &&
                    x.Status == BookingStatus.Pending &&
                    !x.IsDeleted
                )
                .ToListAsync(cancellationToken);

            foreach (var booking in bookings)
            {
                // ❌ Chặn gửi trùng
                var sent = await _context.BookingReminderLog.AnyAsync(
                    x => x.BookingId == booking.Id
                    && x.ReminderConfigId == config.Id,
                    cancellationToken);

                if (sent)
                    continue;

                // ⏰ Booking datetime (UTC)
                var bookingDateTime =
                    booking.BookingDate.Date + booking.BookingTime;

                // ⏰ Thời điểm cần gửi reminder
                // var remindAt =
                //     bookingDateTime.AddMinutes(-config.BeforeMinute);
                var bookingAt = booking.BookingDate.Date + booking.BookingTime;
                var remindAt = bookingAt.AddMinutes(-config.BeforeMinute);

                _logger.LogInformation(
                    "Reminder check | BookingId={BookingId} | Now={Now:o} | BookingAt={BookingAt:o} | RemindAt={RemindAt:o}",
                    booking.Id,
                    now,
                    bookingDateTime,
                    remindAt
                );

                // 👉 Chưa tới giờ
                if (now < remindAt)
                    continue;

                // 👉 Trễ quá 10 phút thì bỏ
               if (now < remindAt)
                    continue;

            // ⛔ QUÁ 10 PHÚT
                if (now > remindAt.AddMinutes(15))
                    continue;

                // 🚀 Send notification
                _logger.LogWarning(
                    "Send notification"
                );
                if (config.Channel == ReminderChannel.PushNotification)
                {
                    _logger.LogWarning(
                        "Booking push noti  Channel={Channel}",
                        config.Channel
                    );
                    await SendPushNotiAsync(booking, cancellationToken);
                }
                // else if (config.Channel == ReminderChannel.Email)
                // {
                //     await SendEmailAsync(booking, cancellationToken);
                // }

                // 📝 Log đã gửi
                _context.BookingReminderLog.Add(new BookingReminderLog
                {
                    BookingId = booking.Id,
                    ReminderConfigId = config.Id,
                    Channel = config.Channel,
                    SentAt = now
                });
            }

            await _context.SaveChangesAsync(cancellationToken);
        }
        return Unit.Value;
    }
    private async Task<bool> SendPushNotiAsync(Booking booking, CancellationToken cancellationToken)
    {
        var devices = new List<AccountDeviceDto>();

        var userDevices = (await _identityClient
            .GetAccountDeviceAsync(booking.UserId, cancellationToken))?.Data;

        var storeDevices = (await _identityClient
            .GetAccountDeviceByStoreIdAsync(booking.StoreId!.Value, cancellationToken))?.Data;

        if (userDevices != null) devices.AddRange(userDevices);
        if (storeDevices != null) devices.AddRange(storeDevices);

        var tokens = devices
            .Select(x => x.Token)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .ToList();

        if (!tokens.Any())
        {
            _logger.LogWarning(
                "No tokens → skip push. BookingId={BookingId}",
                booking.Id
            );
            return false;
        }

        var message = new MulticastMessage
        {
            Tokens = tokens,
            Notification = new FirebaseAdmin.Messaging.Notification
            {
                Title = "Booking Reminder",
                Body = $"You have a schedule {booking.BookingDate:yyyy-MM-dd} {booking.BookingTime}"
            }
        };

        await _firebaseService.SendMulticastAsync(message);

        _logger.LogInformation(
            "Sending booking reminder. BookingId={BookingId}, Tokens={TokenCount}",
            booking.Id,
            tokens.Count);
        return true;
    }

    private async Task SendEmailAsync(Booking booking, CancellationToken cancellationToken)
    {
        // string subject = "Booking Reminder";
        // string body = $"You have a schedule {booking.BookingDate:yyyy-MM-dd} {booking.BookingTime}";
        // await _publishEndpoint.Publish(new SendEmailEvent
        // {
        //     To = entity.Email,
        //     Body = body,
        //     Subject = subject
        // });
    }
}
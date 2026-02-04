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
        var now = DateTime.Now;
        var today = now.Date;
        var configs = await _context.ReminderConfig
            .Where(x => x.IsActive && !x.IsDeleted)
            .ToListAsync(cancellationToken);
        foreach (var config in configs)
        {
            var bookings = await _context.Booking
                .Where(x =>
                    x.StoreId == config.StoreId &&
                    x.BookingDate == today &&
                    x.Status == BookingStatus.Pending &&
                    !x.IsDeleted
                )
                .ToListAsync(cancellationToken);
            foreach (var booking in bookings)
            {
                // Chặn gửi trùng
                var sent = await _context.BookingReminderLog.AnyAsync(
                    x => x.BookingId == booking.Id
                      && x.ReminderConfigId == config.Id,
                    cancellationToken);
                if (sent)
                    continue;
                var bookingDateTime =
                    booking.BookingDate.Date + booking.BookingTime;
                // Thời điểm cần gửi reminder
                var remindAt =
                    bookingDateTime.AddMinutes(-config.BeforeMinute);
                _logger.LogInformation(
                        "Reminder check: BookingId={BookingId}, Now={Now}, RemindAt={RemindAt}",
                        booking.Id,
                        now,
                        remindAt
                );
                if (now < remindAt || now > remindAt.AddMinutes(5))
                    continue;
                if (config.Channel == ReminderChannel.PushNotification)
                {
                    await SendPushAsync(booking, cancellationToken);
                }
                // else if (config.Channel == ReminderChannel.Email)
                // {
                //     await SendEmailAsync(booking, cancellationToken);
                // }
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
    private async Task SendPushAsync(Booking booking, CancellationToken cancellationToken)
    {
        var devices = new List<AccountDeviceDto>();
        try
        {
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
            if (!tokens.Any()) return;
            var message = new MulticastMessage
            {
                Tokens = tokens,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = "Booking Reminder",
                    Body = $"You have a schedule {booking.BookingDate:yyyy-MM-dd} {booking.BookingTime}"
                },
                Data = new Dictionary<string, string>
                {
                    { "BookingId", booking.Id.ToString() },
                    { "Type", "BookingReminder" }
                }
            };
            _logger.LogInformation(
                "Sending booking reminder. BookingId={BookingId}, Tokens={TokenCount}",
                booking.Id,
                tokens.Count);
            await _firebaseService.SendMulticastAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to send booking reminder. BookingId={BookingId}",
                booking.Id);
        }
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
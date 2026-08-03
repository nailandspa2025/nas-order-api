using AutoMapper;
using BuildingBlocks.ApiClients.Clients.AccountDevice.Models;
using BuildingBlocks.ApiClients.Clients.Identity;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Common.Firebase;
using BuildingBlocks.Core.Response;
using FirebaseAdmin.Messaging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Domain.Enums;

namespace Order.Application.Features.Bookings.Commands.CheckinBooking;

public record CheckinBookingCommand : IRequest<ApiResponse>
{
    //public string Phone { get; init; } = default!;
    public int Id { get; init; }
}

public class CheckinBookingCommandHandler : IRequestHandler<CheckinBookingCommand, ApiResponse>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    private readonly IIdentityClient _identityClient;
    private readonly IFirebaseService _firebaseService;

    public CheckinBookingCommandHandler(
        IOrderDbContext context,
        IMapper mapper,
        ICurrentUser currentUser,
        IIdentityClient identityClient,
        IFirebaseService firebaseService)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
        _identityClient = identityClient 
            ?? throw new ArgumentNullException(nameof(identityClient));
        _firebaseService = firebaseService;
    }


    public async Task<ApiResponse> Handle(
        CheckinBookingCommand request,
        CancellationToken cancellationToken)
    {
        if (!long.TryParse(_currentUser.StoreId, out var storeId))
        {
            return ApiResponse.Error("Invalid store.");
        }


        var booking = await _context.Booking
            .Where(x =>
                x.Id == request.Id &&
                x.StoreId == storeId &&
                x.Status == BookingStatus.Pending)
            .FirstOrDefaultAsync(cancellationToken);


        if (booking == null)
        {
            return ApiResponse.Error("No pending booking found.");
        }


        booking.Status = BookingStatus.CheckIn;


        var result = await _context.SaveChangesAsync(cancellationToken);


        if (result <= 0)
        {
            return ApiResponse.Error("Check-in failed.");
        }


        await SendCheckinNotificationAsync(
            booking,
            storeId,
            cancellationToken);


        return ApiResponse.Success(
            "Check-in completed successfully.");
    }



    private async Task SendCheckinNotificationAsync(
        Domain.Entities.Booking booking,
        long storeId,
        CancellationToken cancellationToken)
    {
        try
        {
            var devices = new List<AccountDeviceDto>();


            // Device của user hiện tại
            var userDevices = await _identityClient
                .GetAccountDeviceAsync(
                    _currentUser.UserId,
                    cancellationToken);


            devices.AddRange(
                userDevices?.Data 
                ?? Enumerable.Empty<AccountDeviceDto>());



            // Device theo store
            var storeDevices = await _identityClient
                .GetAccountDeviceByStoreIdAsync(
                    storeId,
                    cancellationToken);


            devices.AddRange(
                storeDevices?.Data 
                ?? Enumerable.Empty<AccountDeviceDto>());



            var title =
                $"Booking Check-in {booking.BookingDate:yyyy-MM-dd} {booking.BookingTime:hh\\:mm}";


            var content =
                $"Booking {booking.Id} has been checked in.";



            var deviceTokens = devices
                .Where(x => !string.IsNullOrEmpty(x.Token))
                .Select(x => x.Token)
                .Distinct()
                .ToList();



            // Firebase Push Notification
            if (deviceTokens.Any())
            {
                await _firebaseService.SendMulticastAsync(
                    new MulticastMessage
                    {
                        Tokens = deviceTokens,

                        Notification = new FirebaseAdmin.Messaging.Notification
                        {
                            Title = title,
                            Body = content
                        },

                        Data = new Dictionary<string, string>
                        {
                            {
                                "ObjectId",
                                booking.Id.ToString()
                            },
                            {
                                "Type",
                                "Booking"
                            }
                        }
                    });
            }



            // Lưu notification history
            _context.Notification.Add(
                new Domain.Entities.Notification
                {
                    AccountId = _currentUser.UserId,
                    Title = title,
                    Content = content,
                    IsRead = false,
                    BookingId = booking.Id,
                    Type = NotificationType.Important
                });


            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Notification lỗi không ảnh hưởng check-in
            Console.WriteLine(
                $"Send check-in notification error: {ex.Message}");
        }
    }
}
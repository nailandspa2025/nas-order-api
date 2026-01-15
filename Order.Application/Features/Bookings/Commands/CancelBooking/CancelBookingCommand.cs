using BuildingBlocks.ApiClients.Clients.AccountDevice.Models;
using BuildingBlocks.ApiClients.Clients.Identity;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Common.Firebase;
using BuildingBlocks.Core.Response;
using FirebaseAdmin.Messaging;
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Application.Features.Bookings.Commands.CancelBooking;

public record CancelBookingCommand: IRequest<ApiResponse>
{
	public int Id { get; init; }

    public int? ReasonId { get; init; }

    public string? Reason { get; init; }
}

public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, ApiResponse>
{
    private readonly IOrderDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IIdentityClient _identityClient;
    private readonly IFirebaseService _firebaseService;

    public CancelBookingCommandHandler (IOrderDbContext context, ICurrentUser currentUser,
        IIdentityClient identityClient,
        IFirebaseService firebaseService)
    {
        _context = context;
        _currentUser = currentUser;
        _firebaseService = firebaseService;
        _identityClient = identityClient ?? throw new ArgumentNullException(nameof(_identityClient));
    }
    public async Task<ApiResponse> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Booking
            .FindAsync(request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Booking), request.Id);
        }
        if(entity.Status != BookingStatus.Pending)
        {
            return ApiResponse.Error("Only cancel bookings with status pending.");
        }
        entity.Status = BookingStatus.Cancelled;
        entity.Reason = request.Reason;
        entity.BookingCancelReasonId = request.ReasonId == 0 ? null : request.ReasonId;

        await _context.SaveChangesAsync(cancellationToken);
        try
        {
            var devices = new List<AccountDeviceDto>();
            var accountDevices = (await _identityClient.GetAccountDeviceAsync(_currentUser.UserId, cancellationToken))?.Data;
            devices.AddRange(accountDevices ?? Enumerable.Empty<AccountDeviceDto>());
            
            if (entity.StoreId.HasValue)
            {
                var storeDeviceResponse = await _identityClient
                    .GetAccountDeviceByStoreIdAsync(entity.StoreId.Value, cancellationToken);

                if (storeDeviceResponse?.Data != null)
                    devices.AddRange(storeDeviceResponse.Data);
            }


            if (entity.BookingTechnicians != null && entity.BookingTechnicians.Any(x => x != null))
            {
                var technicianIds = entity.BookingTechnicians
                .Where(x => x != null)
                .Select(x => x!.ToString())
                .Distinct();
                var accountDeviceResponse = await _identityClient
                    .GetAccountDeviceAsync(string.Join(",", technicianIds), cancellationToken);
                if (accountDeviceResponse?.Data != null)
                    devices.AddRange(accountDeviceResponse.Data);
            }

            //var devices = (await _identityClient.GetAccountDeviceAsync(_currentUser.UserId, cancellationToken))?.Data;
            if (devices != null && devices.Any())
            {
                var deviceTokens = devices.Select(d => d.Token).Distinct().ToList();
                if (deviceTokens.Any())
                {
                    var notifications = new List<Domain.Entities.Notification>();
                    await _firebaseService.SendMulticastAsync(
                        new MulticastMessage()
                        {
                            Tokens = deviceTokens,
                            Notification = new FirebaseAdmin.Messaging.Notification()
                            {
                                Title = $"Cancel {entity.BookingDate:yyyy-MM-dd} {entity.BookingTime}",
                                Body = request.Reason,
                            },
                            Data = new Dictionary<string, string>()
                            {
                                { "ObjectId", entity.Id.ToString() },
                                { "Type", "Booking" },
                            }
                        });

                    notifications.Add(new Domain.Entities.Notification
                    {
                        AccountId = _currentUser.UserId,
                        Title = $"Cancel {entity.BookingDate:yyyy-MM-dd} {entity.BookingTime}",
                        Content = request.Reason,
                        IsRead = false,
                        BookingId = entity.Id,
                        Type = NotificationType.Important
                    });

                    _context.Notification.AddRange(notifications);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }
        }
        catch (Exception)
        {
        }
        return ApiResponse.Success();
    }
}
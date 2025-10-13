using AutoMapper;
using BuildingBlocks.ApiClients.Clients.AccountDevice.Models;
using BuildingBlocks.ApiClients.Clients.Identity;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Common.Firebase;
using BuildingBlocks.Core.Response;
using FirebaseAdmin.Messaging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Bookings.Models;
using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Application.Features.Bookings.Commands.UpdateBooking;

public record UpdateBookingCommand: IRequest<ApiResponse<BookingDto>>
{
    public int Id { get; init; }

    public long? StoreId { get; init; }

    public long? ProductId { get; init; }

    public List<long> TechnicianIds { get; init; } = new List<long>();

    public DateTime BookingDate { get; init; }

    public TimeSpan BookingTime { get; init; }

    public string? Note { get; set; }

    public string FullName { get; init; } = null!;

    public string Address { get; init; } = null!;

    public Gender? Gender { get; init; }

    public string Phone { get; init; } = null!;

    public string Email { get; init; } = null!;

    public int? Number { get; init; }

    public List<int> ServiceIds { get; init; } = new List<int>();

    public List<string> SnapIds { get; init; } = new List<string>();
    public List<string> GroupdIds { get; init; } = new List<string>();
}

public class UpdateBookingCommandHandler : IRequestHandler<UpdateBookingCommand, ApiResponse<BookingDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _curentUser;
    private readonly IIdentityClient _identityClient;
    private readonly IFirebaseService _firebaseService;

    public UpdateBookingCommandHandler(
        IOrderDbContext context,
        IMapper mapper,
        ICurrentUser currentUser,
        IIdentityClient identityClient,
        IFirebaseService firebaseService)
    {
        _mapper = mapper;
        _context = context;
        _curentUser = currentUser;
        _firebaseService = firebaseService;
        _identityClient = identityClient ?? throw new ArgumentNullException(nameof(_identityClient));
    }

    public async Task<ApiResponse<BookingDto>> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
    {
        var bookingServices = new List<BookingService>();
        var bookingTechnicians = new List<BookingTechnician>();

        var bookingSnaps =  new List<BookingSnap>();
        var bookingGroups = new List<BookingSnapGroup>();

        var entity = await _context.Booking
            .Include(x => x.BookingServices)
            .Include(x => x.BookingSnapGroups)
            .Include(x=>x.BookingSnaps)
            .Include(x => x.BookingTechnicians)
            .Where(x=> x.Id == request.Id).FirstOrDefaultAsync();

        if (entity == null)
        {
            throw new NotFoundException(nameof(Booking), request.Id);
        }
        if (entity.Status != BookingStatus.Pending)
        {
            return ApiResponse<BookingDto>.Error("Only update bookings with status pending.");
        }
        var bookingDate = DateTime.SpecifyKind(request.BookingDate.Date, DateTimeKind.Utc);

        entity.StoreId = request.StoreId;
        entity.ProductId = request.ProductId;
        entity.BookingTime = request.BookingTime;
        entity.BookingDate = bookingDate;
        entity.UserId = _curentUser.UserId;
        entity.Note = request.Note;
        entity.FullName = request.FullName;
        entity.Gender = request.Gender;
        entity.Phone = request.Phone;
        entity.Address = request.Address;
        entity.Number = request.Number;
        entity.Email = request.Email;

        if (request.ServiceIds != null && request.ServiceIds.Any())
        {
            bookingServices = request.ServiceIds.Select(id => new BookingService
            {
                ServiceId = id
            }).ToList();
        }
        if (request.TechnicianIds != null && request.TechnicianIds.Any())
        {
             bookingTechnicians = request.TechnicianIds.Select(id => new BookingTechnician
            {
                TechnicianId = id
            }).ToList();
            
        }
        if (request.SnapIds != null && request.SnapIds.Any())
        {
            bookingSnaps = request.SnapIds.Select(s => new BookingSnap
            {
                SnapId = s
            }).ToList();

        }
        if (request.GroupdIds != null && request.GroupdIds.Any())
        {
            bookingGroups = request.GroupdIds.Select(g => new BookingSnapGroup
            {
                GroupdId = g
            }).ToList();
        }
        entity.SetBookingServices(bookingServices);
        entity.SetBookingTechnicians(bookingTechnicians);
        entity.SetBookingSnaps(bookingSnaps);
        entity.SetBookingGroups(bookingGroups);
        await _context.SaveChangesAsync(cancellationToken);

        try
        {
            var devices = new List<AccountDeviceDto>();
            var title = $"Update booking {entity.BookingDate.ToString("yyyy-MM-dd")} {entity.BookingTime.ToString(@"hh\:mm")}";
            if (request.TechnicianIds.Any())
            {
                var accountDeviceResponse = await _identityClient
               .GetAccountDeviceAsync(string.Join(",", request.TechnicianIds), cancellationToken);

                if (accountDeviceResponse?.Data != null)
                    devices.AddRange(accountDeviceResponse.Data);
            }

            if (entity.StoreId.HasValue)
            {
                var storeDeviceResponse = await _identityClient
                    .GetAccountDeviceByStoreIdAsync(entity.StoreId.Value, cancellationToken);

                if (storeDeviceResponse?.Data != null)
                    devices.AddRange(storeDeviceResponse.Data);
            }

            if (devices.Any())
            {
                var deviceTokens = devices.Where(d => !string.IsNullOrWhiteSpace(d.Token)) .Select(d => d.Token).Distinct().ToList();

                if (deviceTokens.Any())
                {
                    await _firebaseService.SendMulticastAsync(new MulticastMessage
                    {
                        Tokens = deviceTokens,
                        Notification = new FirebaseAdmin.Messaging.Notification
                        {
                            Title = title,
                            Body = request.Note
                        },
                        Data = new Dictionary<string, string>
                        {
                            { "ObjectId", entity.Id.ToString()},
                            { "Type", "Booking" },
                            //{ "TestKey", "TestValue" }
                        }
                    });
                }

                var notification = new Domain.Entities.Notification
                {
                    AccountId = _curentUser.UserId,
                    Title = title,
                    Content = request.Note,
                    BookingId = entity.Id,
                    Type = NotificationType.Important,
                    Recipients = devices.Select(d => new NotificationRecipient
                    {
                        UserId = d.AccountId,
                        IsRead = false
                    }).ToList()
                };

                _context.Notification.Add(notification);
                await _context.SaveChangesAsync(cancellationToken);

            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending: {ex.Message}");
        }
        return ApiResponse<BookingDto>.Success(_mapper.Map<BookingDto>(entity));
    }
}
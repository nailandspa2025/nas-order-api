using AutoMapper;
using BuildingBlocks.ApiClients.Clients.AccountDevice.Models;
using BuildingBlocks.ApiClients.Clients.Identity;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Common.Firebase;
using BuildingBlocks.Core.Response;
using FirebaseAdmin.Messaging;
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Bookings.Models;
using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Application.Features.Bookings.Commands.CreateBooking;

public record CreateBookingCommand : IRequest<ApiResponse<BookingDto>>
{

    public long? StoreId { get; init; }

    public long? ProductId { get; init; }

    public List<long> TechnicianIds { get; init; } = new List<long>();

    public DateTime BookingDate { get; init; }

    public TimeSpan BookingTime { get; init; }

    public string? Note { get; set; }

    public string FullName { get; init; } = null!;

    public string? Address { get; init; } = null!;

    public Gender? Gender { get; init; }

    public string Phone { get; init; } = null!;

    public string? Email { get; init; } = null!;

    public int? Number { get; init; }

    public string? UserId { get; init; }

    public List<int> ServiceIds { get; init; } = new List<int>();
    public List<int> ProductIds { get; init; } = new List<int>();
    public List<string> HexColors { get; init; } = new List<string>();
}

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, ApiResponse<BookingDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _curentUser;
    private readonly IIdentityClient _identityClient;
    private readonly IFirebaseService _firebaseService;

    public CreateBookingCommandHandler(
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

    public async Task<ApiResponse<BookingDto>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        //var bookingDate = DateTime.SpecifyKind(request.BookingDate.Date, DateTimeKind.Utc);
        var bookingDate = request.BookingDate.Date;
        var entity = new Booking
        {
            StoreId = request.StoreId,
            ProductId = request.ProductId,
            //TechnicianId = request.TechnicianId,
            BookingTime = request.BookingTime,
            BookingDate = bookingDate,
            Status = BookingStatus.Pending,
            UserId = request.UserId,
            Note = request.Note,
            FullName = request.FullName,
            Gender = request.Gender,
            Phone = request.Phone,
            Address = request.Address,
            Number = request.Number,
            Email = request.Email
        };
        if (request.ServiceIds != null && request.ServiceIds.Any())
        {
            var bookingServices = request.ServiceIds.Select(id => new BookingService
            {
                ServiceId = id
            }).ToList();

            entity.SetBookingServices(bookingServices);
        }
        if (request.TechnicianIds != null && request.TechnicianIds.Any())
        {
            var bookingTechnicians = request.TechnicianIds.Select(id => new BookingTechnician
            {
                TechnicianId = id
            }).ToList();

            entity.SetBookingTechnicians(bookingTechnicians);
        }
        if (request.ProductIds != null && request.ProductIds.Any())
        {
            var bookingProducts = request.ProductIds.Select(id => new BookingProduct
            {
                ProductId = id
            }).ToList();
            entity.SetBookingProducts(bookingProducts);
        }
        if (request.HexColors != null && request.HexColors.Any())
        {
            var bookingColors = request.HexColors.Select(id => new BookingColor
            {
                HexColor = id
            }).ToList();
            entity.SetBookingColors(bookingColors);
        }
        _context.Booking.Add(entity);
        var result = await _context.SaveChangesAsync(cancellationToken);
        if (result > 0)
        {
            try
            {
                var devices = new List<AccountDeviceDto>();
                var accountDevices = (await _identityClient.GetAccountDeviceAsync(request.UserId, cancellationToken))?.Data;
                devices.AddRange(accountDevices ?? Enumerable.Empty<AccountDeviceDto>());
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
                //var devices = (await _identityClient.GetAccountDeviceAsync(request.UserId, cancellationToken))?.Data;
                var title = $"Booking {entity.BookingDate.ToString("yyyy-MM-dd")} {entity.BookingTime.ToString(@"hh\:mm")}";
                if (devices != null && devices.Any())
                {
                    var deviceTokens = devices.Where(d => !string.IsNullOrWhiteSpace(d.Token)).Select(d => d.Token).Distinct().ToList();
                    if (deviceTokens.Any())
                    {
                        // Send push notifications
                        await _firebaseService.SendMulticastAsync(
                            new MulticastMessage()
                            {
                                Tokens = deviceTokens,
                                Notification = new FirebaseAdmin.Messaging.Notification()
                                {
                                    Title = title,
                                    Body = request.Note,
                                },
                                Data = new Dictionary<string, string>()
                                {
                                    { "ObjectId", entity.Id.ToString() },
                                    { "Type", "Booking" },
                                }
                            });

                        // Create a single notification entity
                        var notification = new Domain.Entities.Notification
                        {
                            AccountId = request.UserId,
                            Title = title,
                            Content = request.Note,
                            BookingId = entity.Id,
                            IsRead = false,
                            Type = NotificationType.Booking,
                            Recipients = devices.Select(d => new NotificationRecipient
                            {
                                UserId = d.AccountId,
                                IsRead = false
                            }).ToList()
                        };

                        // Save to database
                        _context.Notification.Add(notification);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending: {ex.Message}");
            }

        }
        return ApiResponse<BookingDto>.Success(_mapper.Map<BookingDto>(entity));
    }
}

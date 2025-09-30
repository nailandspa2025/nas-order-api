using AutoMapper;
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

    public string ? UserId { get; init; }

    public List<int> ServiceIds { get; init; } = new List<int>();
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
        var bookingDate = DateTime.SpecifyKind(request.BookingDate.Date, DateTimeKind.Utc);
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
        _context.Booking.Add(entity);
        var result = await _context.SaveChangesAsync(cancellationToken);
        if (result > 0)
        {
            try
            {
                var devices = (await _identityClient.GetAccountDeviceAsync(request.UserId, cancellationToken))?.Data;
                if (devices != null && devices.Any())
                {
                    var deviceTokens = devices.Select(d => d.Token).ToList();
                    if (deviceTokens.Any())
                    {
                        var notifications = new List<Domain.Entities.Notification>();
                        await _firebaseService.SendMulticastAsync(
                            new MulticastMessage()
                            {
                                Tokens = deviceTokens,
                                Notification = new FirebaseAdmin.Messaging.Notification()
                                {
                                    Title = $"Booking {entity.BookingDate:yyyy-MM-dd} {entity.BookingTime}",
                                    Body = request.Note,
                                },
                                Data = new Dictionary<string, string>()
                                {
                                { "ObjectId", entity.Id.ToString() },
                                { "Type", "Booking" },
                                }
                            });

                        notifications.Add(new Domain.Entities.Notification
                        {
                            AccountId = request.UserId,
                            Title = $"Booking {entity.BookingDate:yyyy-MM-dd} {entity.BookingTime}",
                            Content = request.Note,
                            BookingId = entity.Id,
                            IsRead = false,
                            Type = NotificationType.Booking
                        });

                        _context.Notification.AddRange(notifications);
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                }
            }
            catch (Exception)
            {

            }
        }
        return ApiResponse<BookingDto>.Success(_mapper.Map<BookingDto>(entity));
    }
}

using AutoMapper;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Bookings.Models;
using Order.Domain.Enums;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Core.Response;
using MediatR;
using Order.Domain.Entities;

namespace Order.Application.Features.Bookings.Commands.CreateBooking;

public record CreateBookingCommand : IRequest<ApiResponse<BookingDto>>
{
   
    public long ? StoreId { get; init; }

    public long ? ProductId { get; init; }

    public long ? TechnicianId { get; init; }

    public DateTime BookingDate { get; init; }

    public TimeSpan BookingTime { get; init; }

    public string? Note { get; set; }

    public string? FullName { get; init; }

    public string? Address { get; init; }

    public Gender? Gender { get; init; }

    public string? Phone { get; init; }

    public string? Email { get; init; }

    public int? Number { get; init; }

}

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, ApiResponse<BookingDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _curentUser;

    public CreateBookingCommandHandler(
        IOrderDbContext context,
        IMapper mapper,
        ICurrentUser currentUser)
    {
        _mapper = mapper;
        _context = context;
        _curentUser = currentUser;
    }

    public async Task<ApiResponse<BookingDto>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var entity = new Booking
        {
           StoreId = request.StoreId,
           ProductId = request.ProductId,
           TechnicianId = request.TechnicianId,
           BookingTime = request.BookingTime,
           BookingDate = request.BookingDate,
           Status = BookingStatus.Pending,
           UserId = _curentUser.UserId,
           Note = request.Note,
           FullName = request.FullName,
           Gender = request.Gender,
           Phone = request.Phone,
           Address = request.Address,
           Number = request.Number,
           Email = request.Email
        };

        _context.Booking.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<BookingDto>.Success(_mapper.Map<BookingDto>(entity));
    }
}

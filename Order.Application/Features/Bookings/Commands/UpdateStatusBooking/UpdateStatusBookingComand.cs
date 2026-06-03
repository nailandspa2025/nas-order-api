using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Bookings.Models;
using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Application.Features.Bookings.Commands.UpdateStatusBooking;

public record UpdateStatusBookingCommand : IRequest<ApiResponse<BookingDto>>
{
    public int Id { get; init; }
    public BookingStatus Status { get; init; }
}

public class UpdateStatusBookingCommandHandler : IRequestHandler<UpdateStatusBookingCommand, ApiResponse<BookingDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public UpdateStatusBookingCommandHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<BookingDto>> Handle(UpdateStatusBookingCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Booking
            .FindAsync(request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Booking), request.Id);
        }
        
        entity.Status = request.Status;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<BookingDto>.Success(_mapper.Map<BookingDto>(entity));
    }
}
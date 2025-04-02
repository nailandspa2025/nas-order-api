using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Application.Features.Bookings.Commands.CancelBooking;

public record CancelBookingCommand: IRequest<ApiResponse>
{
	public int Id { get; init; } 
}

public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, ApiResponse>
{
    private readonly IOrderDbContext _context;

    public CancelBookingCommandHandler (IOrderDbContext context)
    {
        _context = context;
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

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();
    }
}
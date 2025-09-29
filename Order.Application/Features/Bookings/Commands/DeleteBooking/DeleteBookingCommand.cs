using Order.Application.Common.Interfaces;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Application.Features.Bookings.Commands.DeleteBooking;

public record DeleteBookingCommand(int Id): IRequest<ApiResponse>;

public class DeleteBookingCommandHandler : IRequestHandler<DeleteBookingCommand, ApiResponse>
{
    private readonly IOrderDbContext _context;

    public DeleteBookingCommandHandler(IOrderDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse> Handle(DeleteBookingCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Booking
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Booking), request.Id);
        }
        if (entity.Status != BookingStatus.Cancelled) 
        {
            return ApiResponse.Error("Only deleted bookings with status canceled.");
        }
        _context.Booking.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();

    }
}


using MediatR;
using Order.Application.Common.Interfaces;

namespace Order.Application.Features.Bookings.Commands.UpdateRateBooking;

public record UpdateRateBookingEvent: IRequest<Unit>
{
    public int Id { get; init; }
    public bool IsRated { get; init; }
}

public class UpdateRateBookingEventHandler : IRequestHandler<UpdateRateBookingEvent, Unit>
{
    private readonly IOrderDbContext _context;

    public UpdateRateBookingEventHandler(IOrderDbContext context)
    {
        _context = context;
    }
    public async Task<Unit> Handle(UpdateRateBookingEvent request, CancellationToken cancellationToken)
    {
        var entity = await _context.Booking
            .FindAsync(request.Id, cancellationToken);

        entity.IsRated = request.IsRated;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}

using MediatR;
using Order.Application.Features.Bookings.Commands.CloseBooking;
namespace Order.Api.JobHangfire;

public class CloseBookingJob
{
    private readonly IMediator _mediator;
    public CloseBookingJob(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task ExecuteAsync()
    {
        await _mediator.Send(new CloseBookingCommand());
    }
}



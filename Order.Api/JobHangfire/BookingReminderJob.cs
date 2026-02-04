using MediatR;
using Order.Application.Features.SendBookingReminder;
namespace Order.Api.JobHangfire;

public class BookingReminderJob
{
    private readonly IMediator _mediator;
     public BookingReminderJob(IMediator mediator)
    {
        _mediator = mediator;
    }
    public async Task ExecuteAsync()
    {
        await _mediator.Send(new SendBookingReminderCommand());
    }
}
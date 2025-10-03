using BuildingBlocks.EventBus;
using BuildingBlocks.EventBus.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using Order.Application.Features.Bookings.Commands.UpdateRateBooking;

namespace Order.Api.Consumer;

public class SentBookingRateEventConsumerDefinition: ConsumerDefinition<SentBookingRateEventConsumer>
{
    public SentBookingRateEventConsumerDefinition(IOptions<AwsBusOptions> options)
    {
        // override the default endpoint name, for whatever reason
        EndpointName = options.Value.Topic.BookingUpdateRate;
    }

    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<SentBookingRateEventConsumer> consumerConfigurator)
    {
        // endpointConfigurator.UseMessageRetry(r => r.Interval(5, 1000));
        endpointConfigurator.UseInMemoryOutbox();
    }
}

public class SentBookingRateEventConsumer : IConsumer<BookingUpdateRateEvent>
{
    private readonly ILogger<SentBookingRateEventConsumer> _logger;
    private readonly IMediator _mediator;

    public SentBookingRateEventConsumer(ILogger<SentBookingRateEventConsumer> logger, IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<BookingUpdateRateEvent> context)
    {
        _logger.LogInformation("Consume Message Update Booking");

        var data = context.Message;

        var command = new UpdateRateBookingEvent
        {
            Id = data.BookingId,
            IsRated = data.IsRated
        };
        await _mediator.Send(command);

        await Task.CompletedTask;
    }
}
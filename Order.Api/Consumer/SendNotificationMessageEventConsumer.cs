using BuildingBlocks.EventBus;
using BuildingBlocks.EventBus.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using Order.Application.Features.Bookings.Commands.PushNotification;

namespace Order.Api.Consumer;

public class SendNotificationMessageEventConsumerDefinition: ConsumerDefinition<SendNotificationMessageEventConsumer>
{
    public SendNotificationMessageEventConsumerDefinition(IOptions<AwsBusOptions> options)
    {
        // override the default endpoint name, for whatever reason
        EndpointName = options.Value.Topic.BookingUpdateRate;
    }

    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<SendNotificationMessageEventConsumer> consumerConfigurator)
    {
        // endpointConfigurator.UseMessageRetry(r => r.Interval(5, 1000));
        endpointConfigurator.UseInMemoryOutbox();
    }
}

public class SendNotificationMessageEventConsumer : IConsumer<PushNotificationEvent>
{
    private readonly ILogger<SendNotificationMessageEventConsumer> _logger;
    private readonly IMediator _mediator;

    public SendNotificationMessageEventConsumer(ILogger<SendNotificationMessageEventConsumer> logger, IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Consume(ConsumeContext<PushNotificationEvent> context)
    {
        _logger.LogInformation("Consume Message push Booking");

        var data = context.Message;

        var command = new PushNotificationMessageEvent
        {
            Content = data.Content,
            UserId = data.UserId
        };
        await _mediator.Send(command);
    }
}
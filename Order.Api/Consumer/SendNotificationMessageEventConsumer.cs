using BuildingBlocks.EventBus;
using BuildingBlocks.EventBus.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Options;
using Order.Application.Features.Bookings.Commands.PushNotification;

namespace Order.Api.Consumer;
public class SendNotificationMessageEventConsumerDefinition
    : ConsumerDefinition<SendNotificationMessageEventConsumer>
{
    public SendNotificationMessageEventConsumerDefinition(IOptions<AwsBusOptions> options)
    {
        EndpointName = options.Value.Topic.SendNotificationMessage;
    }

    protected override void ConfigureConsumer(
        IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<SendNotificationMessageEventConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseInMemoryOutbox();
        endpointConfigurator.UseMessageRetry(r => r.Interval(3, 2000));
    }
}
public class SendNotificationMessageEventConsumer 
    : IConsumer<PushNotificationEvent>
{
    private readonly ILogger<SendNotificationMessageEventConsumer> _logger;
    private readonly IMediator _mediator;

    public SendNotificationMessageEventConsumer(
        ILogger<SendNotificationMessageEventConsumer> logger,
        IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<PushNotificationEvent> context)
    {
        _logger.LogInformation("Consume Push Notification Message");

        var data = context.Message;

        var command = new PushNotificationMessageEvent
        {
            Content = "Test Push Notification",
            UserId = data.UserId
        };
        await _mediator.Send(command);

        await Task.CompletedTask;
    }
}
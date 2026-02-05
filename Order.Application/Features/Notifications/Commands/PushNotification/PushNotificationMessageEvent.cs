using BuildingBlocks.Common.Firebase;
using FirebaseAdmin.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;
using Order.Application.Common.Interfaces;

namespace Order.Application.Features.Bookings.Commands.PushNotification;

public record PushNotificationMessageEvent : IRequest<Unit>
{
    public string UserId { get; init; }
    public string Content { get; init; }
    public List<string> Tokens { get; init; } 
}

public class PushNotificationMessageEventHandler : IRequestHandler<PushNotificationMessageEvent, Unit>
{
    private readonly IOrderDbContext _context;
    private readonly IFirebaseService _firebaseService;
    private readonly ILogger<PushNotificationMessageEventHandler> _logger;

    public PushNotificationMessageEventHandler(IOrderDbContext context, IFirebaseService firebaseService, ILogger<PushNotificationMessageEventHandler> logger)
    {
        _context = context;
        _firebaseService = firebaseService;
        _logger = logger;

    }
    public async Task<Unit> Handle(PushNotificationMessageEvent request, CancellationToken cancellationToken)
    {
            var message = new MulticastMessage
            {
                Tokens = request.Tokens,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = "New message",
                    Body = request.Content
                },
                Data = new Dictionary<string, string>
                {
                    { "ObjectId", request.UserId.ToString() },
                    { "Type", "Message" }
                }
            };
            _logger.LogInformation("Sending push notification to tokens: {Tokens}", request.Tokens);
            await _firebaseService.SendMulticastAsync(message);
        return Unit.Value;
    }
}
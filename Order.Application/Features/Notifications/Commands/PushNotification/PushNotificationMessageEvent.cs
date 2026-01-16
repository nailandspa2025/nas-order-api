using BuildingBlocks.ApiClients.Clients.Identity;
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
}

public class PushNotificationMessageEventHandler : IRequestHandler<PushNotificationMessageEvent, Unit>
{
    private readonly IOrderDbContext _context;
    private readonly IIdentityClient _identityClient;
    private readonly IFirebaseService _firebaseService;
    private readonly ILogger<PushNotificationMessageEventHandler> _logger;

    public PushNotificationMessageEventHandler(IOrderDbContext context, IIdentityClient identityClient, IFirebaseService firebaseService, ILogger<PushNotificationMessageEventHandler> logger)
    {
        _context = context;
        _identityClient = identityClient;
        _firebaseService = firebaseService;
        _logger = logger;

    }
    public async Task<Unit> Handle(PushNotificationMessageEvent request, CancellationToken cancellationToken)
    {
        try
        {
            var accountDevices = (await _identityClient.GetAccountDeviceAsync(request.UserId, cancellationToken))?.Data;
            if (accountDevices.Any())
            {
                var deviceTokens = accountDevices.Where(d => !string.IsNullOrEmpty(d.Token)).Select(d => d.Token).Distinct().ToList();
                if (deviceTokens.Any())
                {
                    await _firebaseService.SendMulticastAsync(new MulticastMessage
                    {
                        Tokens = deviceTokens,
                        Notification = new FirebaseAdmin.Messaging.Notification
                        {
                            Title = "New message",
                            Body = request.Content
                        },
                        Data = new Dictionary<string, string>
                        {
                            { "ObjectId", request.UserId},
                            { "Type", "Message" },
                        }
                    });
                }
            }
        }

        catch (Exception ex)
        {
            _logger.LogWarning("PushNotificationMessageEvent: UserId is empty");
            Console.WriteLine($"Error push notifycation: {ex.Message}");
        }
        return Unit.Value;
    }
}
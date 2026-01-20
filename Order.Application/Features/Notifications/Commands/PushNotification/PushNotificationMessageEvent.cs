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
        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            _logger.LogWarning(
                "PushNotificationMessageEvent ignored: UserId is empty. Content={Content}",
                request.Content
            );
            return Unit.Value;
        }

        try
        {
            var response = await _identityClient.GetAccountDeviceAsync(
                request.UserId,
                cancellationToken
            );

            var accountDevices = response?.Data;

            if (accountDevices == null || !accountDevices.Any())
            {
                _logger.LogInformation(
                    "No device found for UserId={UserId}",
                    request.UserId
                );
                return Unit.Value;
            }

            var deviceTokens = accountDevices
                .Where(d => !string.IsNullOrWhiteSpace(d.Token))
                .Select(d => d.Token!)
                .Distinct()
                .ToList();

            if (!deviceTokens.Any())
            {
                _logger.LogInformation(
                    "No valid device token for UserId={UserId}",
                    request.UserId
                );
                return Unit.Value;
            }
            await _firebaseService.SendMulticastAsync(
                new MulticastMessage()
                {
                    Tokens = deviceTokens,
                    Notification = new FirebaseAdmin.Messaging.Notification()
                    {
                        Title = "New message",
                        Body = request.Content,
                    },
                    Data = new Dictionary<string, string>()
                    {
                        { "ObjectId", request.UserId.ToString() },
                        { "Type", "Message" },
                    }
                });
            _logger.LogInformation(
                "Push notification sent SUCCESS", deviceTokens
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "PushNotificationMessageEvent FAILED. UserId={UserId}",
                request.UserId
            );
        }

        return Unit.Value;
    }
}
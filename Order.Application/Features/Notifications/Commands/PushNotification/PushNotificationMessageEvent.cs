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
            var deviceTokens = new List<string>
            {
                "ckkWLzCV4k3Omu9V_qFhLr:APA91bHayeD4zCBY4bRIJDqFnIPhHpS5U76hbVhR4GsM-s8mv8a_m5AalFjCe3zD-0c9OF9eNlrUbearVbh1zJmVlQHw17Nn9KEdIS2gFbhapWzjgpXWA0Q",
                "cSlkVc9VziTcx66JjdnXly:APA91bF-LxvwSUMtxjTF9O4fU1edgVVMgXdTWilZpgrx_Z37GStb9w8cVM6aSzuonsXvKVFHFGAjoNjwsDL52EsR927g9tqMimuOM0QowCB9WH-ciH4G9NI",
                "dR2PWyXUSYaZUVzsIy8iSe:APA91bE_nGHZHZqX2t7Wci1878gTmyOEDMYHEA77kxEPNsP0nQnQ8skX8b4c8RP9Y63r9Iy33p6QTT0Sh2hJHsiD8U3BQLHzq1x9ulQ6CcIh6AC4EFfPxec"
            };
        
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
                "Push notification sent SUCCESS. TokenCount={Count}",
                deviceTokens.Count
            );
        return Unit.Value;
    }
}
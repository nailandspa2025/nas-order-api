using AutoMapper;
using BuildingBlocks.ApiClients.Clients.Identity;
using BuildingBlocks.Common.Firebase;
using BuildingBlocks.Core.Response;
using FirebaseAdmin.Messaging;
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Notifications.Models;
using Order.Domain.Enums;

namespace Order.Application.Features.Notifications.Commands.CreateNotification;

public record CreateNotificationCommand: IRequest<ApiResponse<NotificationDto>>
{
    public string ? UserId { get; init; } 

    public string? Content { get; init; }

    public string Title { get; set; } = null!;

    public NotificationType Type { get; set; }

}

public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, ApiResponse<NotificationDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly IIdentityClient _identityClient;
    private readonly IFirebaseService _firebaseService;

    public CreateNotificationCommandHandler (IOrderDbContext context, IMapper mapper, IIdentityClient identityClient, IFirebaseService firebaseService )
    {
        _context = context;
        _mapper = mapper;
        _identityClient = identityClient;
        _firebaseService = firebaseService;
    }

    public async  Task<ApiResponse<NotificationDto>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        var entity = new Domain.Entities.Notification
        {
            AccountId = request.UserId,
            Content = request.Content,
            Title = request.Title,
            Type = request.Type,
        };

        _context.Notification.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        try
        {
            var devices = (await _identityClient.GetAccountDeviceAsync(request.UserId, cancellationToken))?.Data;

            if (devices?.Any() == true)
            {
                var deviceTokens = devices.Select(d => d.Token).Where(t => !string.IsNullOrWhiteSpace(t)).ToList();

                if (deviceTokens.Any())
                {
                    var message = new MulticastMessage
                    {
                        Tokens = deviceTokens,
                        Notification = new FirebaseAdmin.Messaging.Notification
                        {
                            Title = request.Title,
                            Body = request.Content
                        },
                        Data = new Dictionary<string, string>
                    {
                        { "ObjectId", entity.Id.ToString() },
                        { "Type", request.Type.ToString() }
                    }
                    };

                    await _firebaseService.SendMulticastAsync(message);
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
        return ApiResponse<NotificationDto>.Success(_mapper.Map<NotificationDto>(entity));
        
    }
}
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Core.Response;
using MediatR;

using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;



namespace Order.Application.Features.Notifications.Commands.UpdateNotification;

public record UpdateReadAllNotificationCommand() : IRequest<ApiResponse>;

public class UpdateReadAllNotificationCommandHandler : IRequestHandler<UpdateReadAllNotificationCommand, ApiResponse>
{
    private readonly IOrderDbContext _context;
    private readonly ICurrentUser _currentUser;
    public UpdateReadAllNotificationCommandHandler(IOrderDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }
    public async Task<ApiResponse> Handle(UpdateReadAllNotificationCommand request, CancellationToken cancellationToken)
    {
        var notifications = await _context.Notification
            .Where(n => n.AccountId == _currentUser.UserId && !n.IsRead)
            .ToListAsync(cancellationToken);
        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();
    }
}


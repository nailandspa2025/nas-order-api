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
        var recipients = await _context.NotificationRecipient
        .Where(r => r.UserId == _currentUser.UserId && !r.IsRead)
        .ToListAsync(cancellationToken);

        foreach (var recipient in recipients)
        {
            recipient.IsRead = true;
        }
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();
    }
}


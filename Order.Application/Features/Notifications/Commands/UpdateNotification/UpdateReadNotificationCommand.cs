using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
namespace Order.Application.Features.Notifications.Commands.UpdateNotification;

public record UpdateReadNotificationCommand (int Id) : IRequest<ApiResponse>;

public class UpdateReadNotificationCommandHandler : IRequestHandler<UpdateReadNotificationCommand, ApiResponse>
{
    private readonly IOrderDbContext _context;
    private readonly ICurrentUser _currentUser;

    public UpdateReadNotificationCommandHandler(IOrderDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse> Handle(UpdateReadNotificationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.NotificationRecipient
            .Where(x => x.NotificationId == request.Id && x.UserId == _currentUser.UserId)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity == null)
        {
            return ApiResponse.Error("Notification not found");
        }
        entity.IsRead = true;
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse.Success();
    }
}


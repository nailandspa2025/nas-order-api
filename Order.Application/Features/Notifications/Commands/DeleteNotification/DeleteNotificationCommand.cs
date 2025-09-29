using Order.Application.Common.Interfaces;
using Order.Domain.Entities;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;

namespace Order.Application.Features.Notifications.Commands.DeleteNotification;

public record DeleteNotificationCommand (int Id): IRequest<ApiResponse>;

public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, ApiResponse>
{
    private readonly IOrderDbContext _context;

    public DeleteNotificationCommandHandler(IOrderDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Notification
            .FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Notification), request.Id);
        }
        _context.Notification.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();

    }
}



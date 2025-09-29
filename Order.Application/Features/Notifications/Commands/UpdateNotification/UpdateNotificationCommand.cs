using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Notifications.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.Notifications.Commands.UpdateNotification;

public record UpdateNotificationCommand: IRequest<ApiResponse<NotificationDto>>
{
    public int Id { get; set; }

    public string ? UserId { get; init; } 

    public string? Content { get; init; }
}

public class UpdateNotificationCommandHandler : IRequestHandler<UpdateNotificationCommand, ApiResponse<NotificationDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public UpdateNotificationCommandHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<NotificationDto>> Handle(UpdateNotificationCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Notification.FindAsync(request.Id, cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(Notification), request.Id);
        }

        entity.AccountId = request.UserId;
        entity.Content = request.Content;
        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<NotificationDto>.Success(_mapper.Map<NotificationDto>(entity));
    }
}
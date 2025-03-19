using AutoMapper;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Notifications.Models;
using Order.Domain.Entities;
using Order.Domain.Enums;
using BuildingBlocks.Core.Response;
using MediatR;

namespace Order.Application.Features.Notifications.Commands.CreateNotification;

public record CreateNotificationCommand: IRequest<ApiResponse<NotificationDto>>
{
    public string UserId { get; init; } = null!;

    public string? Content { get; init; }

    public DateTime SentTime { get; init; }

    public NotificationStatus Status { get; init; }
}

public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, ApiResponse<NotificationDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public CreateNotificationCommandHandler (IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async  Task<ApiResponse<NotificationDto>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        var entity = new Notification
        {
            UserId = request.UserId,
            Content = request.Content,
            Status = request.Status,
            SentTime = request.SentTime,
        };

        _context.Notification.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<NotificationDto>.Success(_mapper.Map<NotificationDto>(entity));
        
    }
}
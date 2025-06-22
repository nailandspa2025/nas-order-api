using AutoMapper;
using BuildingBlocks.Core.Response;
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Notifications.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.Notifications.Commands.CreateNotification;

public record CreateNotificationCommand: IRequest<ApiResponse<NotificationDto>>
{
    public string ? UserId { get; init; } 

    public string? Content { get; init; }

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
            AccountId = request.UserId,
            Content = request.Content,
            IsDeleted = false
        };

        _context.Notification.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<NotificationDto>.Success(_mapper.Map<NotificationDto>(entity));
        
    }
}
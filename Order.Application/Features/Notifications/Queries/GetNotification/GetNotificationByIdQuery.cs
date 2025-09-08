using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Notifications.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.Notifications.Queries.GetNotification;

public class GetNotificationByIdQuery: IRequest<ApiResponse<NotificationDto>>
{
    public int Id { get; init; }
}

public class GetNotificationByIdQueryHandler : IRequestHandler<GetNotificationByIdQuery, ApiResponse<NotificationDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public GetNotificationByIdQueryHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<NotificationDto>> Handle(GetNotificationByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Notification
           .AsNoTracking()
           .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Notification), request.Id);
        }

        return ApiResponse<NotificationDto>.Success(_mapper.Map<NotificationDto>(entity));
    }
}

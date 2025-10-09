using AutoMapper;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;

namespace Order.Application.Features.Notifications.Queries.GetNotificationCount;

public record GetNotificationCountByMeQuery() : IRequest<ApiResponse<int>>;

public class GetNotificationCountByMeQueryHandler : IRequestHandler<GetNotificationCountByMeQuery, ApiResponse<int>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public GetNotificationCountByMeQueryHandler(IOrderDbContext context, ICurrentUser currentUser, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
    }
    public async Task<ApiResponse<int>> Handle(GetNotificationCountByMeQuery request, CancellationToken cancellationToken)
    {
        var count = await _context.Notification
        .Include(n => n.Recipients)
        .Where(n => n.Recipients.Any(r => r.UserId == _currentUser.UserId && !r.IsRead)
                    && !n.IsDeleted)
        .CountAsync(cancellationToken);
        return ApiResponse<int>.Success(count);
    }
}
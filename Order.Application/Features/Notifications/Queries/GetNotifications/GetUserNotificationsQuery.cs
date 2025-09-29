using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Notifications.Models;

namespace Order.Application.Features.Notifications.Queries.GetNotifications;

public record GetUserNotificationsQuery: IRequest<ApiResponse<PaginatedList<NotificationDto>>>
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string? SearchText { get; init; }
}

public class GetUserNotificationsQueryHandler : IRequestHandler<GetUserNotificationsQuery, ApiResponse<PaginatedList<NotificationDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public GetUserNotificationsQueryHandler(IOrderDbContext context, IMapper mapper, ICurrentUser currentUser)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<PaginatedList<NotificationDto>>> Handle(GetUserNotificationsQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = request.SearchText ?? string.Empty;
        var query = _context.Notification.Where(x => x.AccountId == _currentUser.UserId).AsNoTracking();
        if (!paramSearchText.IsNullOrEmpty())
        {
            var lowerSearch = request.SearchText.ToLower();

            query = query.Where(s => s.Title.ToString().ToLower().Contains(lowerSearch) || s.Content.ToString().ToLower().Contains(lowerSearch));
        }

        var paginationResult = await query
            .OrderByDescending(x => x.Created)
            .ProjectTo<NotificationDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<NotificationDto>>.Success(_mapper.Map<PaginatedList<NotificationDto>>(paginationResult));
    }
}

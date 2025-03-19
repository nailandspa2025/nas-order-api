using AutoMapper;
using AutoMapper.QueryableExtensions;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Notifications.Models;
using Order.Domain.Enums;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Order.Application.Features.Notifications.Queries.GetNotificationsWithPagination;

public record GetNotificationsWithPaginationQuery: IRequest<ApiResponse<PaginatedList<NotificationDto>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; init; }
    public NotificationStatus ? Status { get; init; }
}

public class GetNotificationsWithPaginationQueryHandler : IRequestHandler<GetNotificationsWithPaginationQuery, ApiResponse<PaginatedList<NotificationDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public GetNotificationsWithPaginationQueryHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PaginatedList<NotificationDto>>> Handle(GetNotificationsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = request.SearchText ?? string.Empty;
        var query = _context.Notification.AsNoTracking();
        if (!paramSearchText.IsNullOrEmpty())
        {
            var lowerSearch = request.SearchText.ToLower();

            query = query.Where(s => s.Content.ToString().ToLower().Contains(lowerSearch));
        }
        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status);
        }
        var paginationResult = await query
            .OrderBy(x => x.Created)
            .ProjectTo<NotificationDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<NotificationDto>>.Success(paginationResult);
    }
}

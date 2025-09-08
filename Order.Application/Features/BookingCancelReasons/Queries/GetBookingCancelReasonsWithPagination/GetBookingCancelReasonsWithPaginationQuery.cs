using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.BookingCancelReasons.Models;

namespace Order.Application.Features.BookingCancelReasons.Queries.GetBookingCancelReasonsWithPagination;

public record GetBookingCancelReasonsWithPaginationQuery: IRequest<ApiResponse<PaginatedList<BookingCancelReasonDto>>>
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;

    public string? SearchText { get; init; }

    public bool? IsActive { get; init; }
}

public class GetBookingCancelReasonsWithPaginationQueryHandler : IRequestHandler<GetBookingCancelReasonsWithPaginationQuery, ApiResponse<PaginatedList<BookingCancelReasonDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public GetBookingCancelReasonsWithPaginationQueryHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<PaginatedList<BookingCancelReasonDto>>> Handle(GetBookingCancelReasonsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = request.SearchText ?? string.Empty;
        var query = _context.BookingCancelReason.Where(x => !x.IsDeleted).AsNoTracking();
        if (!paramSearchText.IsNullOrEmpty())
        {
            var lowerSearch = request.SearchText.ToLower();
            query = query.Where(s => s.Name.ToLower().Contains(lowerSearch));
        }
        if (request.IsActive.HasValue)
        {
            query = query.Where(x => x.IsActive == request.IsActive.Value);
        }
        var paginationResult = await query
           .OrderBy(x => x.Created)
           .ProjectTo<BookingCancelReasonDto>(_mapper.ConfigurationProvider)
           .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<BookingCancelReasonDto>>.Success(paginationResult);
    }
}

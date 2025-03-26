using AutoMapper;
using AutoMapper.QueryableExtensions;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Bookings.Models;
using Order.Domain.Enums;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Order.Application.Features.Bookings.Queries.GetBookingsWithPagination;

public record GetBookingsWithPaginationQuery : IRequest<ApiResponse<PaginatedList<BookingDto>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; init; }
    public BookingStatus ? Status { get; init; }
}

public class GetBookingsWithPaginationQueryHandler : IRequestHandler<GetBookingsWithPaginationQuery, ApiResponse<PaginatedList<BookingDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public GetBookingsWithPaginationQueryHandler (IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PaginatedList<BookingDto>>> Handle(GetBookingsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = request.SearchText ?? string.Empty;
        var query = _context.Booking.AsNoTracking();
        if (!paramSearchText.IsNullOrEmpty())
        {
            var lowerSearch = request.SearchText.ToLower();

            query = query.Where(s => s.Id.ToString().ToLower().Contains(lowerSearch));
        }
        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status);
        }
        var paginationResult = await query
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Created)
            .ProjectTo<BookingDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<BookingDto>>.Success(paginationResult);
    }
}

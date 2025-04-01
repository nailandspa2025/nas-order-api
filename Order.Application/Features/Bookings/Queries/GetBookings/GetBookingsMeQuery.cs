using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Bookings.Models;

namespace Order.Application.Features.Bookings.Queries.GetBookings;

public record GetBookingsMeQuery : IRequest<ApiResponse<PaginatedList<BookingDto>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; init; }

}

public class GetBookingsMeQueryHandler : IRequestHandler<GetBookingsMeQuery, ApiResponse<PaginatedList<BookingDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public GetBookingsMeQueryHandler(IOrderDbContext context, IMapper mapper, ICurrentUser currentUser)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<PaginatedList<BookingDto>>> Handle(GetBookingsMeQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = request.SearchText ?? string.Empty;
        var query = _context.Booking.AsNoTracking();
        if (!paramSearchText.IsNullOrEmpty())
        {
            var lowerSearch = request.SearchText.ToLower();

            query = query.Where(s => s.FullName.ToLower().Contains(lowerSearch)
            || s.Phone.Contains(lowerSearch)
            || s.Email.ToLower().Contains(lowerSearch));
        }
        
        var paginationResult = await query
            .Where(x => !x.IsDeleted && x.UserId == _currentUser.UserId )
            .OrderBy(x => x.Created)
            .ProjectTo<BookingDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<BookingDto>>.Success(paginationResult);
    }
}


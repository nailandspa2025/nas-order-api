using AutoMapper;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Bookings.Models;

namespace Order.Application.Features.Bookings.Queries.GetBookings;

public record GetBookingByStoreIdsQuery: IRequest<ApiResponse<IEnumerable<BookingDto>>>
{
    public string StoreIds { get; init; } = null!;
}

public class GetBookingByStoreIdsQueryHandler : IRequestHandler<GetBookingByStoreIdsQuery, ApiResponse<IEnumerable<BookingDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public GetBookingByStoreIdsQueryHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<BookingDto>>> Handle(GetBookingByStoreIdsQuery request, CancellationToken cancellationToken)
    {
        var storeIds = request.StoreIds.Split(",");

        var bookings = await _context.Booking
            .AsNoTracking()
            .Where(x => storeIds.Contains(x.StoreId.ToString()))
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<BookingDto>>.Success(_mapper.Map<IEnumerable<BookingDto>>(bookings));
    }
}
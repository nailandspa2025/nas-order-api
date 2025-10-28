using AutoMapper;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Bookings.Models;
using Order.Domain.Enums;

namespace Order.Application.Features.Bookings.Queries.GetBookings;

public record GetBookingTechnicianByStoreIdQuery: IRequest<ApiResponse<IEnumerable<BookingDto>>>
{
    public int StoreId { get; init; }

    public int TechnicianId { get; init; }

    public DateTime Date { get; init; }
}

public class GetBookingTechnicianByStoreIdQueryHandler : IRequestHandler<GetBookingTechnicianByStoreIdQuery, ApiResponse<IEnumerable<BookingDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public GetBookingTechnicianByStoreIdQueryHandler (IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<IEnumerable<BookingDto>>> Handle(GetBookingTechnicianByStoreIdQuery request, CancellationToken cancellationToken)
    {
        var utcDate = DateTime.SpecifyKind(request.Date, DateTimeKind.Utc);

        var bookings = await _context.Booking
            .Include(x => x.BookingTechnicians)
            .Where(x => x.StoreId == request.StoreId 
            && x.BookingDate.Date == utcDate.Date
            && !x.IsDeleted
            && x.Status != BookingStatus.Cancelled
            && x.BookingTechnicians.Any(bt => bt.TechnicianId == request.TechnicianId))
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<BookingDto>>.Success(_mapper.Map<IEnumerable<BookingDto>>(bookings));
    }
}
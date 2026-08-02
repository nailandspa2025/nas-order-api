using AutoMapper;
using BuildingBlocks.Authentication.Abstractions;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Bookings.Models;
using Order.Domain.Enums;

namespace Order.Application.Features.Bookings.Queries.GetWalkinBookings;

public record GetPendingWalkInBookingsByPhoneQuery : IRequest<ApiResponse<IEnumerable<BookingDto>>>
{
    public string Phone { get; init; } = default!;
}

public class GetPendingWalkInBookingsByPhoneQueryHandler : IRequestHandler<GetPendingWalkInBookingsByPhoneQuery, ApiResponse<IEnumerable<BookingDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;
    public GetPendingWalkInBookingsByPhoneQueryHandler(IOrderDbContext context, IMapper mapper, ICurrentUser currentUser)
    {
        _context = context;
        _mapper = mapper;
        _currentUser = currentUser;
    }
    public async Task<ApiResponse<IEnumerable<BookingDto>>> Handle(GetPendingWalkInBookingsByPhoneQuery request, CancellationToken cancellationToken)
    {
        var today = DateTime.Now.Date;
        var bookings = await _context.Booking.Where(b => b.Phone == request.Phone
            && b.Status == BookingStatus.Pending
            && b.BookingDate == today
            && b.StoreId == Convert.ToInt64(_currentUser.StoreId)
            ).ToListAsync(cancellationToken);
        var bookingDtos = _mapper.Map<IEnumerable<BookingDto>>(bookings);
        
        return ApiResponse<IEnumerable<BookingDto>>.Success(bookingDtos);
    }
}
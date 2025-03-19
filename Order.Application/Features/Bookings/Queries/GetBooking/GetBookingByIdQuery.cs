using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Bookings.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.Bookings.Queries.GetBooking;

public record GetBookingByIdQuery: IRequest<ApiResponse<BookingDto>>
{
    public long Id { get; init; }
}

public class GetBookingByIdQueryHandler : IRequestHandler<GetBookingByIdQuery, ApiResponse<BookingDto>>
{
    private readonly IOrderDbContext _context;
    private IMapper _mapper;
    public GetBookingByIdQueryHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<BookingDto>> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Booking
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Booking), request.Id);
        }

        return ApiResponse<BookingDto>.Success(_mapper.Map<BookingDto>(entity));
    }
}

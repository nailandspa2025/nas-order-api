using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Bookings.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.Bookings.Commands.UpdateRateBooking;

public record UpdateRateBookingCommand: IRequest<ApiResponse<BookingDto>>
{
    public int Id { get; init; }
    public bool IsRated { get; init; }
}

public class UpdateRateBookingCommandHandler : IRequestHandler<UpdateRateBookingCommand, ApiResponse<BookingDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public UpdateRateBookingCommandHandler(
        IOrderDbContext context,
        IMapper mapper
        )
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<BookingDto>> Handle(UpdateRateBookingCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Booking
            .FindAsync(request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Booking), request.Id);
        }
        entity.IsRated = request.IsRated;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<BookingDto>.Success(_mapper.Map<BookingDto>(entity));
    }
}

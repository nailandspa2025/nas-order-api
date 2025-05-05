using AutoMapper;
using BuildingBlocks.Core.Response;
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Application.Features.BookingCancelReasons.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.BookingCancelReasons.Commands.CreateBookingCancelReason;

public record CreateBookingCancelReasonCommand : IRequest<ApiResponse<BookingCancelReasonDto>>
{
	public string Name { get; init; } = null!;

	public bool IsActive { get; init; } 
}

public class CreateBookingCancelReasonCommandHandler : IRequestHandler<CreateBookingCancelReasonCommand, ApiResponse<BookingCancelReasonDto>>
{
    private readonly IMapper _mapper;
    private readonly IOrderDbContext _context;

    public CreateBookingCancelReasonCommandHandler( IMapper mapper, IOrderDbContext context)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<BookingCancelReasonDto>> Handle(CreateBookingCancelReasonCommand request, CancellationToken cancellationToken)
    {
        var entity = new BookingCancelReason
        {
            Name = request.Name,
            IsActive = request.IsActive
        };
        _context.BookingCancelReason.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<BookingCancelReasonDto>.Success(_mapper.Map<BookingCancelReasonDto>(entity));
    }
}

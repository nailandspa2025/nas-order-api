using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Application.Features.BookingCancelReasons.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.BookingCancelReasons.Commands.UpdateBookingCancelReason;

public record UpdateBookingCancelReasonCommand:IRequest<ApiResponse<BookingCancelReasonDto>>
{
	public int Id { get; init; }

	public string Name { get; init; } = null!;

	public bool IsActive { get; init; }

}

public class UpdateBookingCancelReasonCommandHandler : IRequestHandler<UpdateBookingCancelReasonCommand, ApiResponse<BookingCancelReasonDto>>
{
    private readonly IMapper _mapper;
    private readonly IOrderDbContext _context;

    public UpdateBookingCancelReasonCommandHandler(IMapper mapper, IOrderDbContext context)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<BookingCancelReasonDto>> Handle(UpdateBookingCancelReasonCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.BookingCancelReason

            .FindAsync(request.Id, cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(BookingCancelReason), request.Id);
        }
        entity.Name = request.Name;
        entity.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);
        return ApiResponse<BookingCancelReasonDto>.Success(_mapper.Map<BookingCancelReasonDto>(entity));
    }
}

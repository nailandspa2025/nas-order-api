using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Order.Application.Common.Interfaces;
using Order.Domain.Entities;

namespace Order.Application.Features.BookingCancelReasons.Commands.DeleteBookingCancelReason;

public record DeleteBookingCancelReasonCommand(int Id) : IRequest<ApiResponse>;

public class DeleteBookingCancelReasonCommandHandler : IRequestHandler<DeleteBookingCancelReasonCommand, ApiResponse>
{
    private readonly IOrderDbContext _context;

    public DeleteBookingCancelReasonCommandHandler( IOrderDbContext context)
    {
        _context = context;
    }

    public async Task<ApiResponse> Handle(DeleteBookingCancelReasonCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.BookingCancelReason
            .FindAsync(new object[] { request.Id }, cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(BookingCancelReason), request.Id);
        }
        _context.BookingCancelReason.Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse.Success();
    }
}

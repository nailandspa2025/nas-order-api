using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.BookingCancelReasons.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.BookingCancelReasons.Queries.GetBookingCancelReason;

public record GetBookingCancelReasonByIdQuery:IRequest<ApiResponse<BookingCancelReasonDto>>
{
    public int Id { get; init; }
}

public class GetBookingCancelReasonByIdQueryHandler : IRequestHandler<GetBookingCancelReasonByIdQuery, ApiResponse<BookingCancelReasonDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public GetBookingCancelReasonByIdQueryHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<BookingCancelReasonDto>> Handle(GetBookingCancelReasonByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.BookingCancelReason
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(BookingCancelReason), request.Id);
        }
        return ApiResponse<BookingCancelReasonDto>.Success(_mapper.Map<BookingCancelReasonDto>(entity));
    }
}

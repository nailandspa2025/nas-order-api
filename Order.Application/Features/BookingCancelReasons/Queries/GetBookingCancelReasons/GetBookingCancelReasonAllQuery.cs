using AutoMapper;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.BookingCancelReasons.Models;

namespace Order.Application.Features.BookingCancelReasons.Queries.GetBookingCancelReasons;

public record GetBookingCancelReasonAllQuery: IRequest<ApiResponse<IEnumerable<BookingCancelReasonDto>>>;

public class GetBookingCancelReasonAllQueryHandler : IRequestHandler<GetBookingCancelReasonAllQuery, ApiResponse<IEnumerable<BookingCancelReasonDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public GetBookingCancelReasonAllQueryHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<BookingCancelReasonDto>>> Handle(GetBookingCancelReasonAllQuery request, CancellationToken cancellationToken)
    {
        var reasons = await _context.BookingCancelReason
            .Where(x => !x.IsDeleted && x.IsActive )
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<BookingCancelReasonDto>>.Success(_mapper.Map<IEnumerable<BookingCancelReasonDto>>(reasons));
    }
}
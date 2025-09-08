using AutoMapper;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.BookingCancelReasons.Models;

namespace Order.Application.Features.BookingCancelReasons.Queries.GetBookingCancelReasons;

public record GetBookingCancelReasonByIdsQuery: IRequest<ApiResponse<IEnumerable<BookingCancelReasonDto>>>
{
	public string Ids { get; init; } = null!;
}

public class GetBookingCancelReasonByIdsQueryHandler : IRequestHandler<GetBookingCancelReasonByIdsQuery, ApiResponse<IEnumerable<BookingCancelReasonDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public GetBookingCancelReasonByIdsQueryHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<BookingCancelReasonDto>>> Handle(GetBookingCancelReasonByIdsQuery request, CancellationToken cancellationToken)
    {
        var ids = request.Ids.Split(",");

        var reasons = await _context.BookingCancelReason
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id.ToString()))
            .ToListAsync(cancellationToken);

        return ApiResponse<IEnumerable<BookingCancelReasonDto>>.Success(_mapper.Map<IEnumerable<BookingCancelReasonDto>>(reasons));
    }
}
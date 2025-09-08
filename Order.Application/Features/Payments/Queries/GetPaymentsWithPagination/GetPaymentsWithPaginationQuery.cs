using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Common.Extensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Payments.Models;

namespace Order.Application.Features.Payments.Queries.GetPaymentsWithPagination;

public record GetPaymentsWithPaginationQuery: IRequest<ApiResponse<PaginatedList<PaymentDto>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchText { get; init; }
}

public class GetPaymentsWithPaginationQueryHandler : IRequestHandler<GetPaymentsWithPaginationQuery, ApiResponse<PaginatedList<PaymentDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public GetPaymentsWithPaginationQueryHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<PaginatedList<PaymentDto>>> Handle(GetPaymentsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paramSearchText = request.SearchText ?? string.Empty;

        var query = _context.Payment.AsNoTracking();
        if (!paramSearchText.IsNullOrEmpty())
        {
            query = query.Where(s => paramSearchText.ToLower().Contains(s.Amount.ToString().ToLower())
                            || paramSearchText.ToLower().Contains(s.FullName.ToString().ToLower())
                            ||paramSearchText.ToLower().Contains(s.Phone.ToString().ToLower())
                            || paramSearchText.ToLower().Contains(s.Email.ToString().ToLower())
                            || paramSearchText.ToLower().Contains(s.BookingId.ToString().ToLower())
                            );
        }

        var paginationResult = await query
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Created)
            .ProjectTo<PaymentDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<PaymentDto>>.Success(paginationResult);
    }
}
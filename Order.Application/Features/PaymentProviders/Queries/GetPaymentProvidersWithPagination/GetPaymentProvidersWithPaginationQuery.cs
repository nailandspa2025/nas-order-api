using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.Common.Mappings;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.PaymentProviders.Models;

namespace Order.Application.Features.PaymentProviders.Queries.GetPaymentProvidersWithPagination;

public record GetPaymentProvidersWithPaginationQuery : IRequest<ApiResponse<PaginatedList<PaymentProviderDto>>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public PaymentMethod ? PaymentMethod { get; init; }
}

public class GetPaymentProvidersWithPaginationQueryHandler : IRequestHandler<GetPaymentProvidersWithPaginationQuery, ApiResponse<PaginatedList<PaymentProviderDto>>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    public GetPaymentProvidersWithPaginationQueryHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<PaginatedList<PaymentProviderDto>>> Handle(GetPaymentProvidersWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var query = _context.PaymentProvider.Where(x => !x.IsDeleted).AsNoTracking();
        if (request.PaymentMethod.HasValue)
        {
            query = query.Where(s => s.PaymentMethod == request.PaymentMethod);
        }
        
        var paginationResult = await query
            .OrderBy(x => x.Created)
            .Include(x => x.PaymentProviderSettings)
            .ProjectTo<PaymentProviderDto>(_mapper.ConfigurationProvider)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return ApiResponse<PaginatedList<PaymentProviderDto>>.Success(paginationResult);
    }
}

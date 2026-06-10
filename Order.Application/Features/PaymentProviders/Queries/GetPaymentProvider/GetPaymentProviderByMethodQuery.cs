
using AutoMapper;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.PaymentProviders.Models;

namespace Order.Application.Features.PaymentProviders.Queries.GetPaymentProvider;

public record GetPaymentProviderByMethodQuery : IRequest<ApiResponse<PaymentProviderDto>>
{
     public PaymentMethod PaymentMethod { get; init; }
}

public class GetPaymentProviderByMethodQueryHandler : IRequestHandler<GetPaymentProviderByMethodQuery, ApiResponse<PaymentProviderDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
     public GetPaymentProviderByMethodQueryHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<PaymentProviderDto>> Handle(GetPaymentProviderByMethodQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.PaymentProvider
            .Include(x => x.PaymentProviderSettings)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PaymentMethod == request.PaymentMethod, cancellationToken: cancellationToken);
        return ApiResponse<PaymentProviderDto>.Success(_mapper.Map<PaymentProviderDto>(entity));
    }
}
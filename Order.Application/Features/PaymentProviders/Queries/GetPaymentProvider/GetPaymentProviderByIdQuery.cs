using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.PaymentProviders.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.PaymentProviders.Queries.GetPaymentProvider;

public record GetPaymentProviderByIdQuery: IRequest<ApiResponse<PaymentProviderDto>>
{
    public long Id { get; init; }
}

public class GetPaymentProviderByIdQueryHandler : IRequestHandler<GetPaymentProviderByIdQuery, ApiResponse<PaymentProviderDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    public GetPaymentProviderByIdQueryHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<ApiResponse<PaymentProviderDto>> Handle(GetPaymentProviderByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.PaymentProvider
            .Include(x => x.PaymentProviderSettings)
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken: cancellationToken);
        if (entity == null)
        {
            throw new NotFoundException(nameof(PaymentProvider), request.Id);
        }
        return ApiResponse<PaymentProviderDto>.Success(_mapper.Map<PaymentProviderDto>(entity));
    }
}
using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Payments.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.Payments.Queries.GetPayment;

public record GetPaymentByIdQuery: IRequest<ApiResponse<PaymentDto>>
{
	public int Id { get; init; }
}

public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, ApiResponse<PaymentDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public GetPaymentByIdQueryHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PaymentDto>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.Payment
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Payment), request.Id);
        }
        return ApiResponse<PaymentDto>.Success(_mapper.Map<PaymentDto>(entity));
    }
}
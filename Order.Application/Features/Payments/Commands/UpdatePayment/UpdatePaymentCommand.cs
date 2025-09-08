using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Payments.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.Payments.Commands.UpdatePayment;

public record UpdatePaymentCommand: IRequest<ApiResponse<PaymentDto>>
{
    public int Id { get; init; }

    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaidAt { get; set; }
}

public class UpdatePaymentCommandHandler : IRequestHandler<UpdatePaymentCommand, ApiResponse<PaymentDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public UpdatePaymentCommandHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PaymentDto>> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
    {
        
        var entity = await _context.Payment
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Payment), request.Id);
        }
        entity.Amount = request.Amount;
        entity.PaidAt = request.PaidAt;

        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<PaymentDto>.Success(_mapper.Map<PaymentDto>(entity));
    }
}
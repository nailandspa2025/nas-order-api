using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Payments.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.Payments.Commands.CretePayment;

public record CreatePaymentCommand: IRequest<ApiResponse<PaymentDto>>
{
    public int BookingId { get; set; } 

    public decimal Amount { get; set; }

    public DateTime PaidAt { get; set; }
}

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, ApiResponse<PaymentDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;

    public CreatePaymentCommandHandler(IOrderDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PaymentDto>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Booking
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.BookingId, cancellationToken);

        if (booking == null)
        {
            throw new NotFoundException(nameof(Booking), request.BookingId);
        }
        var entity = new Payment
        {
            BookingId = request.BookingId,
            Amount = request.Amount,
            PaidAt = request.PaidAt
        };

        _context.Payment.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return ApiResponse<PaymentDto>.Success(_mapper.Map<PaymentDto>(entity));
    }
}
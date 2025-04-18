using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Payments.Models;
using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Application.Features.Payments.Commands.CretePayment;

public record CreatePaymentCommand: IRequest<ApiResponse<PaymentDto>>
{
    public int BookingId { get; set; } 

    public decimal Amount { get; set; }

    public PaymentMethod Method { get; init; }

    public string? FullName { get; init; }

    public string? Email { get; init; }

    public string? Phone { get; init; }

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
            .SingleOrDefaultAsync(x => x.Id == request.BookingId, cancellationToken);

        if (booking == null)
        {
            throw new NotFoundException(nameof(Booking), request.BookingId);
        }

        if (booking.Status == BookingStatus.Completed)
        {
            return ApiResponse<PaymentDto>.Error("Booking has been paid");
        }
        var payment = new Payment
        {
            BookingId = request.BookingId,
            Amount = request.Amount,
            PaidAt = DateTime.UtcNow,
            Status = PaymentStatus.Pending,
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone
        };
        var transaction = new Transaction
        {
            Payment = payment,
            Amount = request.Amount,
            Status = TransactionStatus.Success,
            ProcessedAt = DateTime.UtcNow
        };
        switch (request.Method)
        {
            case PaymentMethod.Cash:
                payment.Status = PaymentStatus.Success;
                transaction.Provider = "Cash";
                transaction.TransactionId = $"CASH-{Guid.NewGuid()}";
                break;

            case PaymentMethod.BankTransfer:
                payment.Status = PaymentStatus.Success;
                transaction.Provider = "BankTransfer";
                transaction.TransactionId = $"BANK-{Guid.NewGuid()}";
                break;

            default:
                return ApiResponse<PaymentDto>.Error("Invalid payment method.");
        }

        booking.Status = BookingStatus.Completed;

        await _context.Payment.AddAsync(payment, cancellationToken);
        await _context.Transaction.AddAsync(transaction, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<PaymentDto>(payment);
        return ApiResponse<PaymentDto>.Success(result);
    }
}
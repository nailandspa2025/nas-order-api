using AutoMapper;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using BuildingBlocks.EventBus.Events;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Payments.Models;
using Order.Domain.Enums;

namespace Order.Application.Features.Payments.Commands.CaptureStripe;

public record CaptureStripeCommand : IRequest<ApiResponse<PaymentDto>>
{
    public string SessionId { get; init; } = string.Empty;
    public int BookingId { get; init; }
}

public class CaptureStripeCommandHandler : IRequestHandler<CaptureStripeCommand, ApiResponse<PaymentDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMapper _mapper;

    public CaptureStripeCommandHandler(
        IOrderDbContext context,
        IPublishEndpoint publishEndpoint,
        IMapper mapper)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
        _mapper = mapper;
    }
    public async Task<ApiResponse<PaymentDto>> Handle(CaptureStripeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Booking
            .Include(x => x.Payments)
            .ThenInclude(x => x.Transactions)
            .SingleOrDefaultAsync(
                x => x.Id == request.BookingId,
                cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(
                nameof(PaymentDto),
                request.BookingId);
        }

        var payment = entity.Payments.FirstOrDefault();

        if (payment == null)
        {
            return ApiResponse<PaymentDto>.Error("Payment not found.");
        }

        var transaction = payment.Transactions.FirstOrDefault();

        if (transaction == null)
        {
            return ApiResponse<PaymentDto>.Error("Transaction not found.");
        }

        if (payment.Status == PaymentStatus.Success)
        {
            return ApiResponse<PaymentDto>.Error(
                "Payment has already been captured.");
        }

        payment.Status = PaymentStatus.Success;
        payment.PaidAt = DateTime.UtcNow;

        transaction.Status = TransactionStatus.Success;
        transaction.ProcessedAt = DateTime.UtcNow;

        entity.Status = BookingStatus.Completed;

        await _publishEndpoint.Publish(new BookingPaidEvent
        {
            BookingId = entity.Id,
            StoreId = entity.StoreId ?? 0,
            AccountId = entity.UserId,
            Amount = payment.Amount,
            Process = (int)LoyaltyProcess.Payment
        });

        await _context.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<PaymentDto>(payment);

        return ApiResponse<PaymentDto>.Success(result);
    }
}

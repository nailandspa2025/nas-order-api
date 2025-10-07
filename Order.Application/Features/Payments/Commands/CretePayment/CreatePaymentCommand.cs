using AutoMapper;
using BuildingBlocks.ApiClients.Clients.Catalog;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using BuildingBlocks.EventBus.Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Payments.Models;
using Order.Application.Features.Payments.Services.Paypal;
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

    public string ReturnUrl { get; init; }

    public string CancelUrl { get; init; }

}

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, ApiResponse<PaymentDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICatalogClient _catalogClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreatePaymentCommandHandler(IOrderDbContext context, IMapper mapper, ICatalogClient catalogClient, IHttpContextAccessor httpContextAccessor, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _mapper = mapper;
        _catalogClient = catalogClient;
        _httpContextAccessor = httpContextAccessor;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<ApiResponse<PaymentDto>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Booking
            .Include(x => x.Payments)
            .SingleOrDefaultAsync(x => x.Id == request.BookingId, cancellationToken);

        if (booking == null)
        {
            throw new NotFoundException(nameof(Booking), request.BookingId);
        }

        if (booking.Status == BookingStatus.Completed)
        {
            return ApiResponse<PaymentDto>.Error("Booking has been paid");
        }
        if (booking.StoreId == null)
        {
            return ApiResponse<PaymentDto>.Error("StoreId is required for PayPal payment.");
        }
        //var existingPayment = booking.Payments
        //                .FirstOrDefault(p => p.BookingId == request.BookingId 
        //                && p.Status == PaymentStatus.Pending
        //                && !string.IsNullOrEmpty(p.ApproveUrl)
        //                );

        //if (existingPayment != null)
        //{
        //    var dto = _mapper.Map<PaymentDto>(existingPayment);
        //    return ApiResponse<PaymentDto>.Success(dto);
        //}

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
            Status = TransactionStatus.Pending,
            ProcessedAt = DateTime.UtcNow
        };
        switch (request.Method)
        {
            case PaymentMethod.Paypal:
                var returnUrl = $"{request.ReturnUrl}?bookingId={booking.Id}";
                var cancelUrl = $"{request.CancelUrl}?bookingId={booking.Id}";
                var response = await _catalogClient.GetPaypalConfigAsync(booking.StoreId.Value);
                var config = response?.Data;
                if (config == null)
                {
                    ApiResponse<PaymentDto>.Error($"PaypalConfig not found for StoreId={booking.StoreId}");
                }
                var paypalService = new PaypalService(config);
                var order = await paypalService.CreateOrderAsync(request.Amount, returnUrl, cancelUrl);
                var approveUrl = order.Links.FirstOrDefault(l => l.Rel == "approve")?.Href ?? "";

                payment.ApproveUrl = approveUrl;
                payment.Status = PaymentStatus.Pending;
                transaction.Status = TransactionStatus.Pending;
                transaction.Provider = "PayPal";
                transaction.TransactionId = order.Id;
                break;
            case PaymentMethod.Cash:
                payment.Status = PaymentStatus.Success;
                transaction.Status = TransactionStatus.Success;
                transaction.Provider = "Cash";
                transaction.TransactionId = $"CASH-{Guid.NewGuid()}";
                booking.Status = BookingStatus.Completed;
                break;

            case PaymentMethod.BankTransfer:
                payment.Status = PaymentStatus.Success;
                transaction.Status = TransactionStatus.Success;
                transaction.Provider = "BankTransfer";
                transaction.TransactionId = $"BANK-{Guid.NewGuid()}";
                booking.Status = BookingStatus.Completed;
                break;

            default:
                return ApiResponse<PaymentDto>.Error("Invalid payment method.");
        }

        await _context.Payment.AddAsync(payment, cancellationToken);
        await _context.Transaction.AddAsync(transaction, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _publishEndpoint.Publish(new BookingPaidEvent
        {
            BookingId = booking.Id,
            StoreId = (long)booking.StoreId,
            AccountId = booking.UserId,
            Amount = request.Amount,
            Process = (int)LoyaltyProcess.Payment
        });

        var result = _mapper.Map<PaymentDto>(payment);
        
        return ApiResponse<PaymentDto>.Success(result);
    }
}
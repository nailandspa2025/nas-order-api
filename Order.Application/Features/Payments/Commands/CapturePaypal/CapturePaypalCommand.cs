using AutoMapper;
using BuildingBlocks.ApiClients.Clients.Catalog;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Core.Response;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Payments.Models;
using Order.Application.Features.Payments.Services.Paypal;
using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Application.Features.Payments.Commands.CapturePaypal;

public record CapturePaypalCommand: IRequest<ApiResponse<PaymentDto>>
{
    public string OrderId { get; set; } = null!;
    public int BookingId { get; set; }
}

public class CapturePaypalCommandHandler : IRequestHandler<CapturePaypalCommand, ApiResponse<PaymentDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICatalogClient _catalogClient;

    public CapturePaypalCommandHandler(IOrderDbContext context, IMapper mapper, ICatalogClient catalogClient)
    {
        _context = context;
        _mapper = mapper;
        _catalogClient = catalogClient;
    }

    public async Task<ApiResponse<PaymentDto>> Handle(CapturePaypalCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Booking
            .Include(x => x.Payments)
            .ThenInclude(p => p.Transactions)
            .SingleOrDefaultAsync(x => x.Id == request.BookingId, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Booking), request.BookingId);
        }
        var payment = entity.Payments.FirstOrDefault();
        if (payment == null)
        {
            return ApiResponse<PaymentDto>.Error("Payment not found for booking.");
        }
        var transaction = payment.Transactions.FirstOrDefault();
        if (transaction == null)
        {
            return ApiResponse<PaymentDto>.Error("Transaction not found for payment.");
        }
        if (entity.StoreId == null)
        {
            return ApiResponse<PaymentDto>.Error("StoreId is required for PayPal payment.");
        }
        var response = await _catalogClient.GetPaypalConfigAsync(entity.StoreId.Value);
        var config = response?.Data;
        if (config == null)
        {
            return ApiResponse<PaymentDto>.Error($"PaypalConfig not found for StoreId={entity.StoreId}");
        }

        var paypalService = new PaypalService(config);
        var order = await paypalService.CaptureOrderAsync(request.OrderId);

        if (order.Status == "COMPLETED")
        {
            payment.Status = PaymentStatus.Success;
            payment.PaidAt = DateTime.UtcNow;

            transaction.Status = TransactionStatus.Success;
            transaction.ProcessedAt = DateTime.UtcNow;
            transaction.TransactionId = order.Id;

            entity.Status = BookingStatus.Completed;
        }
        else
        {
            payment.Status = PaymentStatus.Failed;
            transaction.Status = TransactionStatus.Failed;
        }

        await _context.SaveChangesAsync(cancellationToken);

        var result = _mapper.Map<PaymentDto>(payment);
        return ApiResponse<PaymentDto>.Success(result);
    }
}

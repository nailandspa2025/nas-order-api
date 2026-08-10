using AutoMapper;
using BuildingBlocks.ApiClients.Clients.AccountDevice.Models;
using BuildingBlocks.ApiClients.Clients.Catalog;
using BuildingBlocks.ApiClients.Clients.Identity;
using BuildingBlocks.Common.Exceptions;
using BuildingBlocks.Common.Firebase;
using BuildingBlocks.Core.Response;
using BuildingBlocks.EventBus.Events;
using FirebaseAdmin.Messaging;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Order.Application.Common.Interfaces;
using Order.Application.Features.Payments.Models;
using Order.Application.Features.Payments.Services.Paypal;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Stripe;
using Stripe.Checkout;

namespace Order.Application.Features.Payments.Commands.CretePayment;

public record CreatePaymentCommand : IRequest<ApiResponse<PaymentDto>>
{
    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public PaymentMethod Method { get; init; }

    public string? FullName { get; init; }

    public string? Email { get; init; }

    public string? Phone { get; init; }

    public string ReturnUrl { get; init; }

    public string CancelUrl { get; init; }
    public decimal ServiceAmount { get; init; }
    // Giảm giá
    public decimal DiscountAmount { get; init; }
    // Phụ thu
    public decimal SurchargeAmount { get; init; }
    // Tiền khách đưa (tiền mặt)
    public decimal? CustomerPaid { get; init; }
    // Tiền thối lại
    public decimal? ChangeAmount { get; init; }
    
    public decimal? TipAmount { get; set; }
    public decimal? Percentage { get; set; } // optional
    public TipType TipType { get; set; }
    public List<TipAllocationRequest> TipAllocations { get; init; } = new List<TipAllocationRequest>();

}
public class TipAllocationRequest
{
    public decimal TechnicianRevenue { get; init; }
    public decimal TipAmount { get; init; }
    public long TechnicianId { get; init; }
    public TipAllocationType AllocationType { get; init; }
}
public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, ApiResponse<PaymentDto>>
{
    private readonly IOrderDbContext _context;
    private readonly IMapper _mapper;
    private readonly ICatalogClient _catalogClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IIdentityClient _identityClient;
    private readonly IFirebaseService _firebaseService;


    public CreatePaymentCommandHandler(IOrderDbContext context, IMapper mapper, ICatalogClient catalogClient, IHttpContextAccessor httpContextAccessor, IPublishEndpoint publishEndpoint, IIdentityClient identityClient, IFirebaseService firebaseService)
    {
        _context = context;
        _mapper = mapper;
        _catalogClient = catalogClient;
        _httpContextAccessor = httpContextAccessor;
        _publishEndpoint = publishEndpoint;
        _identityClient = identityClient;
        _firebaseService = firebaseService;
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
        var payment = new Payment
        {
            BookingId = request.BookingId,
            Amount = request.Amount,
            PaidAt = DateTime.UtcNow,
            Status = PaymentStatus.Pending,
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            ServiceAmount = request.ServiceAmount,
            DiscountAmount = request.DiscountAmount,
            SurchargeAmount = request.SurchargeAmount,
            CustomerPaid = request.CustomerPaid,
            ChangeAmount = request.ChangeAmount,
            TipAmount = request.TipAmount,
            Percentage = request.Percentage,
            TipType = request.TipType
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
                var paypalResponse = await _catalogClient.GetPaymentProviderAsync(booking.StoreId!.Value,  (int)PaymentMethod.Paypal);
                var providerPaypal = paypalResponse?.Data;
                if (providerPaypal == null)
                {
                    ApiResponse<PaymentDto>.Error($"PaypalConfig not found for StoreId={booking.StoreId}");
                }
                var config = new PaypalConfigDto
                {
                    ClientId = providerPaypal.GetValue("ClientId") ?? string.Empty,
                    ClientSecret = providerPaypal.GetValue("ClientSecret") ?? string.Empty,
                    Currency = providerPaypal.GetValue("Currency") ?? "USD",
                    IsSandbox = bool.TryParse(
                        providerPaypal.GetValue("IsSandbox"),
                        out var sandbox)
                            ? sandbox
                            : true
                };
                var paypalService = new PaypalService(config);
                var order = await paypalService.CreateOrderAsync(request.Amount, returnUrl, cancelUrl);
                var approveUrl = order.Links.FirstOrDefault(l => l.Rel == "approve")?.Href ?? "";

                payment.ApproveUrl = approveUrl;
                payment.Status = PaymentStatus.Pending;
                transaction.Status = TransactionStatus.Pending;
                transaction.Provider = "PayPal";
                transaction.TransactionId = order.Id;
                break;
            case PaymentMethod.Stripe:
                var stripeResponse = await _catalogClient.GetPaymentProviderAsync(booking.StoreId!.Value, (int)PaymentMethod.Stripe);
                var providerStripe = stripeResponse?.Data;
                if (providerStripe == null)
                {
                    return ApiResponse<PaymentDto>.Error( $"Stripe config not found for StoreId={booking.StoreId}");
                }
                var publishableKey = providerStripe.GetValue("PublishableKey");
                var secretKey = providerStripe.GetValue("SecretKey");
                var webhookSecret = providerStripe.GetValue("WebhookSecret");
                if (string.IsNullOrWhiteSpace(secretKey))
                {
                    return ApiResponse<PaymentDto>.Error("Stripe SecretKey not configured.");
                }
                 StripeConfiguration.ApiKey = secretKey;
                var sessionOptions = new SessionCreateOptions
                {
                    Mode = "payment",
                    SuccessUrl = $"{request.ReturnUrl}?bookingId={booking.Id}",
                    CancelUrl = $"{request.CancelUrl}?bookingId={booking.Id}",
                    PaymentMethodTypes = new List<string>
                    {
                        "card"
                    },
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new(){
                            Quantity = 1,
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                Currency = "usd",

                                UnitAmount = (long)(request.Amount * 100),
                                ProductData =
                                    new SessionLineItemPriceDataProductDataOptions
                                    {
                                        Name = $"Booking #{booking.Id}"
                                    }
                            }
                        }
                    },
                    Metadata = new Dictionary<string, string>
                    {
                        { "BookingId", booking.Id.ToString() },
                        { "PaymentId", payment.Id.ToString() }
                    }
                };
                var sessionService = new SessionService();
                var session = await sessionService.CreateAsync(sessionOptions);
                payment.ApproveUrl = session.Url;
                payment.Status = PaymentStatus.Pending;
                transaction.Status = TransactionStatus.Pending;
                transaction.Provider = "Stripe";
                transaction.TransactionId = session.Id;

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
        if (request.TipAmount > 0 && request.TipAllocations.Any())
        {
            var tipAllocations =
                request.TipAllocations
                    .Select(x => new TipAllocation
                    {
                        PaymentId = payment.Id,

                        TechnicianRevenue =
                            x.TechnicianRevenue,

                        TipAmount =
                            x.TipAmount,

                        TechnicianId =
                            x.TechnicianId,

                        AllocationType =
                            x.AllocationType
                    })
                    .ToList();

            await _context.TipAllocation.AddRangeAsync(
                tipAllocations,
                cancellationToken);

            await _context.SaveChangesAsync(
                cancellationToken);
        }
        if (payment.Status == PaymentStatus.Success)
        {
            try
            {
                var notificationDevices = new List<AccountDeviceDto>();
                var userDeviceResponse = await _identityClient
                    .GetAccountDeviceAsync(booking.UserId, cancellationToken);

                notificationDevices.AddRange(
                    userDeviceResponse?.Data ?? Enumerable.Empty<AccountDeviceDto>()
                );
                // Store devices (optional – nếu muốn báo cho store)
                if (booking.StoreId.HasValue)
                {
                    var storeDeviceResponse = await _identityClient
                        .GetAccountDeviceByStoreIdAsync(booking.StoreId.Value, cancellationToken);

                    notificationDevices.AddRange(
                        storeDeviceResponse?.Data ?? Enumerable.Empty<AccountDeviceDto>()
                    );
                }
               if (booking.BookingTechnicians != null && booking.BookingTechnicians.Any(x => x != null))
                {
                    var technicianIds = booking.BookingTechnicians
                    .Where(x => x != null)
                    .Select(x => x!.ToString())
                    .Distinct();

                    var accountDeviceResponse = await _identityClient
                        .GetAccountDeviceAsync(string.Join(",", technicianIds), cancellationToken);
                    if (accountDeviceResponse?.Data != null)
                        notificationDevices.AddRange(accountDeviceResponse.Data);
                }
                var notificationTokens = notificationDevices
                    .Where(d => !string.IsNullOrWhiteSpace(d.Token))
                    .Select(d => d.Token)
                    .Distinct()
                    .ToList();
                if (notificationTokens.Any())
                {
                    var notifications = new List<Domain.Entities.Notification>();
                    var notificationTitle = $"Payment {booking.BookingDate.ToString("yyyy-MM-dd")} {booking.BookingTime.ToString(@"hh\:mm")}";
                    var notificationBody =$"Your booking #{booking.Id} has been paid successfully.";
                    await _firebaseService.SendMulticastAsync(
                            new MulticastMessage()
                            {
                                Tokens = notificationTokens,
                                Notification = new FirebaseAdmin.Messaging.Notification()
                                {
                                    Title = notificationTitle,
                                    Body = notificationBody,
                                },
                                Data = new Dictionary<string, string>()
                                {
                                { "ObjectId", booking.Id.ToString() },
                                { "Type", "Booking" },
                                }
                            });

                        notifications.Add(new Domain.Entities.Notification
                        {
                            AccountId = booking.UserId,
                            Title = notificationTitle,
                            Content = notificationBody,
                            IsRead = false,
                            BookingId = booking.Id,
                            Type = NotificationType.Important
                        }); 
                }
                await _publishEndpoint.Publish(new BookingPaidEvent
                {
                    BookingId = booking.Id,
                    StoreId = (long)booking.StoreId,
                    AccountId = booking.UserId,
                    Amount = request.Amount,
                    Process = (int)LoyaltyProcess.Payment
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Payment notification error: {ex.Message}");
            }
        }

        var result = _mapper.Map<PaymentDto>(payment);
        
        return ApiResponse<PaymentDto>.Success(result);
    }
}
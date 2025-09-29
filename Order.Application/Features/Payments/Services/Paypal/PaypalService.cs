using BuildingBlocks.ApiClients.Clients.Catalog;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System.Globalization;

namespace Order.Application.Features.Payments.Services.Paypal;

public class PaypalService
{
    private readonly PayPalHttpClient _client;
    private readonly PaypalConfigDto _config;

    public PaypalService(PaypalConfigDto config)
    {
        _config = config;
        _client = PaypalClientFactory.CreateClient(config);
    }
    public async Task<PayPalCheckoutSdk.Orders.Order> CreateOrderAsync(decimal amount, string returnUrl, string cancelUrl)
    {
        var orderRequest = new OrdersCreateRequest();
        orderRequest.Prefer("return=representation");
        orderRequest.RequestBody(new OrderRequest()
        {
            CheckoutPaymentIntent = "CAPTURE",
            PurchaseUnits = new List<PurchaseUnitRequest>
        {
            new PurchaseUnitRequest
            {
                AmountWithBreakdown = new AmountWithBreakdown
                {
                    CurrencyCode = _config.Currency,
                    Value = amount.ToString("F2", CultureInfo.InvariantCulture)
                }
            }
        },
            ApplicationContext = new ApplicationContext
            {
                ReturnUrl = returnUrl,
                CancelUrl = cancelUrl
            }
        });

        var response = await _client.Execute(orderRequest);
        return response.Result<PayPalCheckoutSdk.Orders.Order>();
    }
    public async Task<PayPalCheckoutSdk.Orders.Order> CaptureOrderAsync(string orderId)
    {
        var request = new OrdersCaptureRequest(orderId);
        request.Prefer("return=representation");
        request.RequestBody(new OrderActionRequest());

        var response = await _client.Execute(request);
        return response.Result<PayPalCheckoutSdk.Orders.Order>();
    }
}

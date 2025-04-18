namespace BuildingBlocks.PaymentGateway;

public class ZaloPayPaymentService: IPaymentService
{
    public Task<string> ProcessPaymentAsync(decimal amount, string bookingId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> RefundAsync(string transactionId, decimal amount)
    {
        throw new NotImplementedException();
    }
}


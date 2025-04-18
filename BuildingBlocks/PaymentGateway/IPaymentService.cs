namespace BuildingBlocks.PaymentGateway;

public interface IPaymentService
{
    Task<string> ProcessPaymentAsync(decimal amount, string bookingId);
    Task<bool> RefundAsync(string transactionId, decimal amount);
}


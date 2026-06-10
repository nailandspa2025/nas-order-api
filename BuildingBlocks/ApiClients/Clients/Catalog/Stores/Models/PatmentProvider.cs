namespace BuildingBlocks.ApiClients.Clients.Catalog;

public class PaymentProviderDto
{
    public long Id { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public bool IsActive { get; set; }

    public List<PaymentProviderSettingDto> Settings { get; set; } = new();

    public string? GetValue(string key)
    {
        return Settings
            .FirstOrDefault(x =>
                x.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
            ?.Value;
    }
}

public class PaymentProviderSettingDto
{
    public long Id { get; set; }

    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;
}

public enum PaymentMethod
{
    Cash = 1,
    CreditCard = 2,
    Momo = 3,
    Zalopay = 4,
    BankTransfer = 5,
    VNPay = 7,
    Paypal = 8,
    Stripe = 9,
}
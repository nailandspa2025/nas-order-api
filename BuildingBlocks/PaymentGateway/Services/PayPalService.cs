
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace BuildingBlocks.PaymentGateway.Services;
public class PayPalService : IPayPalService
{
    private readonly HttpClient _httpClient;
    private readonly PayPalSettings _settings;

    public PayPalService(HttpClient httpClient, IOptions<PayPalSettings> settings)
    {
        _httpClient = httpClient;
        _settings = settings.Value;
    }
    private async Task<string> GetAccessTokenAsync()
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.BaseUrl}/v1/oauth2/token");
        var byteArray = Encoding.UTF8.GetBytes($"{_settings.ClientId}:{_settings.ClientSecret}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        request.Content = new StringContent("grant_type=client_credentials", Encoding.UTF8, "application/x-www-form-urlencoded");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("access_token").GetString();
    }
    public async Task<string> CreateOrderAsync(decimal amount, string currency, string returnUrl, string cancelUrl)
    {
        var token = await GetAccessTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var body = new
        {
            intent = "CAPTURE",
            purchase_units = new[]
            {
                new {
                    amount = new { currency_code = currency, value = amount.ToString("F2") }
                }
            },
            application_context = new
            {
                return_url = returnUrl,
                cancel_url = cancelUrl
            }
        };

        var response = await _httpClient.PostAsJsonAsync($"{_settings.BaseUrl}/v2/checkout/orders", body);
        var json = await response.Content.ReadAsStringAsync();

        var doc = JsonDocument.Parse(json);
        return doc.RootElement.GetProperty("id").GetString()!;
    }

    public async Task<bool> CaptureOrderAsync(string orderId)
    {
        var token = await GetAccessTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.PostAsync($"{_settings.BaseUrl}/v2/checkout/orders/{orderId}/capture", null);

        return response.IsSuccessStatusCode;
    }
}

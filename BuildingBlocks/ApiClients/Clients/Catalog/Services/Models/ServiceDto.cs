namespace BuildingBlocks.ApiClients.Clients.Catalog.Services.Models;

public class ServiceDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public TimeSpan? WorkingTime { get; set; }
    public string? UrlImage { get; set; }
    public decimal PriceTo { get; set; }
    public decimal PriceFrom { get; set; }
    public int Commission { get; set; }
    public int CommissionType { get; set; }
    
}

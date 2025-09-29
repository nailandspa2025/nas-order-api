namespace BuildingBlocks.ApiClients.Clients.Catalog.ServicePackages.Models;

public class ServicePackageDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public string? Description { get; set; }

    public decimal Price { get; set; }

    public int DurationDays { get; set; }
}

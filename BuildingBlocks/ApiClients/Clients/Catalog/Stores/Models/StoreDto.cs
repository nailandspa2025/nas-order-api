namespace BuildingBlocks.ApiClients.Clients.Catalog.Stores.Models;

public class StoreDto
{
	public long Id { get; set; }

	public string? StoreName { get; set; }

	public string? Hotline { get; set; }

	public string? AddressStore { get; set; }

	public string? Avatar { get; set; }

	public string Email { get; set; }

	public int RatingStar { get; set; }

	public TimeSpan OpenTime { get; set; }

	public TimeSpan CloseTime { get; set; }

	public string TimeZone { get; set; }
	public bool IsCommission { get; set; }
	public bool IsRevenue { get; set; }
}


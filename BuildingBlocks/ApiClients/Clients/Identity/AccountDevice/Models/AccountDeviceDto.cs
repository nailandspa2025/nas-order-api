namespace BuildingBlocks.ApiClients.Clients.AccountDevice.Models;

public class AccountDeviceDto
{
	public int Id { get; set; }

    public string? DeviceId { get; set; }

    public string? AccountId { get; set; }

    public string? Token { get; set; }

    public string? OsVersion { get; set; }

    public string? OperatingSystem { get; set; }

    public string? Platform { get; set; }

    public string? AppName { get; set; }

    public string? Manufacturer { get; set; }
}


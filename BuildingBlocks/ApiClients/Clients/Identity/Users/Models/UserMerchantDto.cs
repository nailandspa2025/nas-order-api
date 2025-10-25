namespace BuildingBlocks.ApiClients.Clients.Identity.Users.Models;

public class UserMerchantDto
{
    public string Id { get; set; } = null!;

    public List<long> StoreIds { get; set; }

    public bool IsOwner { get; set; }

}


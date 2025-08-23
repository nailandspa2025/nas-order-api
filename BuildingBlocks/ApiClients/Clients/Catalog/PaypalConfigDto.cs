namespace BuildingBlocks.ApiClients.Clients.Catalog
{
    public class PaypalConfigDto
    {
        public string ClientId { get; set; } 
        public string ClientSecret { get; set; }
        public string Currency { get; set; } = "USD";
        public bool IsSandbox { get; set; }
    }
}

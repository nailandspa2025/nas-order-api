namespace BuildingBlocks.ApiClients.Clients.Identity.Users.Models;

public class AppAccountDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Avatar { get; set; }
    public string? Street { get; set; }

}

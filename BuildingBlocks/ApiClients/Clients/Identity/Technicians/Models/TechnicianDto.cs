using System;
namespace BuildingBlocks.ApiClients.Clients.Identity.Technicians.Models;

public class TechnicianDto
{
    public long Id { get; set; }

    public string? TechnicianName { get; set; }

    public string? Phone { get; set; }

    public string? TechnicianAddress { get; set; }
    public string? Avatar { get; set; }

}


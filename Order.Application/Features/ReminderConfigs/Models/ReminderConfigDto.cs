using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.ReminderConfigs.Models;

public class ReminderConfigDto: BaseAuditableDto
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int StoreId { get; set; }

    public ReminderChannel Channel { get; set; }

    public int BeforeMinute { get; set; }

    public bool IsActive { get; set; }

    public string? StoreName { get; set; }
    
    private class Mapping: Profile
    {
        public Mapping()
        {
            CreateMap<ReminderConfig, ReminderConfigDto>();
        }
    }
}
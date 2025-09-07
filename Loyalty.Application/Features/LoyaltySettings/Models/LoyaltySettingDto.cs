using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Loyalty.Domain.Entities;

namespace Loyalty.Application.Features.LoyaltySettings.Models;

public class LoyaltySettingDto: BaseAuditableDto<int>
{
    private class Mapping : Profile
    {
        public String Name { get; set; } = null!;
        public bool IsDraft { get; set; }
        public Mapping()
        {
            CreateMap<LoyaltySetting, LoyaltySettingDto>();
        }
    }
}


using BuildingBlocks.Persistence.EntityFrameworkCore;
using Loyalty.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Loyalty.Application.Common.Interfaces;


public interface ILoyaltyDbContext : IEfCoreDbContext
{
    DbSet<LoyaltySetting> LoyaltySetting { get; }
    DbSet<LoyaltyGroup> LoyaltyGroup { get; }
    DbSet<LoyaltyProgram> LoyaltyProgram { get; }
    DbSet<LoyaltyConfigPoint> LoyaltyConfigPoint { get; }
    DbSet<LoyaltyConfigTier> LoyaltyConfigTier { get; }
    DbSet<LoyaltyTier> LoyaltyTier { get; }
}


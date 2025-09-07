using BuildingBlocks.Persistence.EntityFrameworkCore;
using Loyalty.Application.Common.Interfaces;
using Loyalty.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Loyalty.Infrastructure.Persistence;

public class LoyaltyDbContext: EfCoreDbContext<LoyaltyDbContext>, ILoyaltyDbContext
{
    public LoyaltyDbContext(
        DbContextOptions<LoyaltyDbContext> options)
        : base(options)
    {
    }
    public DbSet<LoyaltySetting> LoyaltySetting => Set<LoyaltySetting>();
    public DbSet<LoyaltyGroup> LoyaltyGroup => Set<LoyaltyGroup>();
    public DbSet<LoyaltyProgram> LoyaltyProgram => Set<LoyaltyProgram>();
    public DbSet<LoyaltyConfigPoint> LoyaltyConfigPoint => Set<LoyaltyConfigPoint>();
    public DbSet<LoyaltyConfigTier> LoyaltyConfigTier => Set<LoyaltyConfigTier>();
    public DbSet<LoyaltyTier> LoyaltyTier => Set<LoyaltyTier>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}


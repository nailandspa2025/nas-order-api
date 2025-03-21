using System.Reflection;
using Order.Application.Common.Interfaces;
using Order.Domain.Entities;
using BuildingBlocks.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Order.Infrastructure.Persistence;

public class OrderDbContext: EfCoreDbContext<OrderDbContext>, IOrderDbContext
{
    public OrderDbContext(
        DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    public DbSet<Booking> Booking => Set<Booking>();

    public DbSet<Notification> Notification => Set<Notification>();

    public DbSet<Payment> Payment => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}


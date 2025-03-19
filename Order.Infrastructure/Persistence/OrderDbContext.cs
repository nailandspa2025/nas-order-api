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

    public DbSet<Order.Domain.Entities.Booking> Booking => Set<Order.Domain.Entities.Booking>();

    public DbSet<Notification> Notification => Set<Notification>();


    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}


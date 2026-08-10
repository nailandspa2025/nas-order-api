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

    public DbSet<Transaction> Transaction => Set<Transaction>();

    public DbSet<BookingCancelReason> BookingCancelReason =>Set<BookingCancelReason>();

    public DbSet<BookingService> BookingService =>Set<BookingService>();

    public DbSet<BookingTechnician> BookingTechnician => Set<BookingTechnician>();

    public DbSet<BookingSnap> BookingSnap => Set<BookingSnap>();

    public DbSet<BookingSnapGroup> BookingSnapGroup => Set<BookingSnapGroup>();

    public DbSet<NotificationRecipient> NotificationRecipient => Set<NotificationRecipient>();
    public DbSet<ReminderConfig> ReminderConfig => Set<ReminderConfig>();

    public DbSet<BookingReminderLog> BookingReminderLog => Set<BookingReminderLog>(); 
    public DbSet<BookingTechnicianService> BookingTechnicianService => Set<BookingTechnicianService>();
    public DbSet<PaymentProviderSetting> PaymentProviderSetting => Set<PaymentProviderSetting>();
    public DbSet<PaymentProvider> PaymentProvider => Set<PaymentProvider>();
    public DbSet<TipAllocation> TipAllocation => Set<TipAllocation>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}


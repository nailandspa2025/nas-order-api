
using BuildingBlocks.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Order.Domain.Entities;

namespace Order.Application.Common.Interfaces;


public interface IOrderDbContext : IEfCoreDbContext
{
    DbSet<Booking> Booking { get; }

    DbSet<Payment> Payment { get; }

    DbSet<Notification> Notification { get; }

    DbSet<Transaction> Transaction { get; }

    DbSet<BookingCancelReason> BookingCancelReason { get; }

    DbSet<BookingService> BookingService { get; }

    DbSet<BookingTechnician> BookingTechnician { get; }

    DbSet<BookingSnap> BookingSnap { get; }

    DbSet<BookingSnapGroup> BookingSnapGroup { get; }

    DbSet<NotificationRecipient> NotificationRecipient { get; }
    DbSet<ReminderConfig> ReminderConfig { get; }

    DbSet<BookingReminderLog> BookingReminderLog { get; }
    DbSet<BookingTechnicianService> BookingTechnicianService { get; }

}


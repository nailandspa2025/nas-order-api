
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
}


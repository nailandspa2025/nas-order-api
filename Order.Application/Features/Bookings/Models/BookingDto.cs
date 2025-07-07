using AutoMapper;
using Order.Domain.Enums;
using BuildingBlocks.Persistence.Models;
using Order.Domain.Entities;
using BuildingBlocks.ApiClients.Clients.Catalog.Stores.Models;
using BuildingBlocks.ApiClients.Clients.Identity.Technicians.Models;

namespace Order.Application.Features.Bookings.Models;

public class BookingDto: BaseAuditableDto
{
    public int Id { get; set; }

    public long? StoreId { get; set; }

    public long? ProductId { get; set; }

    public long? TechnicianId { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime BookingDate { get; set; }

    public TimeSpan BookingTime { get; set; }

    public BookingStatus Status { get; set; }

    public PaymentStatus? PaymentStatus { get; set; }

    public PaymentMethod? PaymentMethod { get; set; }

    public string? Note { get; set; }

    public string? FullName { get; set; }

    public string? Address { get; set; }

    public Gender? Gender { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public int? Number { get; set; }

    public int TransactionId { get; set; }

    public virtual Payment? Payment { get; set; }

    public StoreDto? Store { get; set; }

    public TechnicianDto ? Technician { get; set; }

    public int ServicePackageId { get; set; }

    public int SnapId { get; set; }

    private class Mapping: Profile
    {
        public Mapping()
        {
            CreateMap<Booking, BookingDto>();
        }
    }
}


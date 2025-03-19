using AutoMapper;
using Order.Domain.Enums;
using BuildingBlocks.Persistence.Models;

namespace Order.Application.Features.Bookings.Models;

public class BookingDto: BaseAuditableDto
{
    public int Id { get; set; }

    public long StoreId { get; set; }

    public long ProductId { get; set; }

    public long TechnicianId { get; set; }

    public string UserId { get; set; } = null!;

    public DateTime BookingTime { get; set; }

    public BookingStatus Status { get; set; }

    public PaymentStatus PaymentStatus { get; set; }

    public PaymentMethod PaymentMethod { get; set; }

    public int TransactionId { get; set; }

    private class Mapping: Profile
    {
        public Mapping()
        {
            CreateMap<Order.Domain.Entities.Booking, BookingDto>();
        }
    }
}


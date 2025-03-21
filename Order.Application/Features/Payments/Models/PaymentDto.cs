using AutoMapper;
using BuildingBlocks.Persistence.Entities.Common;
using Order.Domain.Entities;

namespace Order.Application.Features.Payments.Models;

public class PaymentDto: BaseAuditableEntity<int>
{
    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaidAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Payment, PaymentDto>();
        }
    }
}

using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.Payments.Models;

public class PaymentDto: BaseAuditableDto
{
    public int Id { get; set; }

    public int BookingId { get; set; }

    public decimal Amount { get; set; }

    public DateTime PaidAt { get; set; }

    public PaymentStatus Status { get; set; }

    public string? FullName { get; init; }

    public string? Email { get; init; }

    public string? Phone { get; init; }

    public string? ApproveUrl { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Payment, PaymentDto>();
        }
    }
}

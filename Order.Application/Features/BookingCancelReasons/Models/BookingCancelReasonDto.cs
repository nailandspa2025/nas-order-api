using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.BookingCancelReasons.Models;

public class BookingCancelReasonDto: BaseAuditableDto
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<BookingCancelReason, BookingCancelReasonDto>();
        }
    }
}
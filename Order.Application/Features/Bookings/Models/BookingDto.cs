using AutoMapper;
using BuildingBlocks.ApiClients.Clients.Catalog.Services.Models;
using BuildingBlocks.ApiClients.Clients.Catalog.Stores.Models;
using BuildingBlocks.ApiClients.Clients.Identity.Technicians.Models;
using BuildingBlocks.Persistence.Models;
using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Application.Features.Bookings.Models;

public class BookingDto: BaseAuditableDto
{
    public int Id { get; set; }

    public long? StoreId { get; set; }

    public long? ProductId { get; set; }

    public List<long> TechnicianIds { get; set; } = new List<long>();

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

    //public List<TechnicianDto> ? Technicians { get; set; }

    public List<int> ServiceIds { get; set; } = new List<int>();

    //public List<ServiceDto>? Services { get; set; }

    public List<string> GroupdIds { get; set; } = new List<string>();
    public List<string> SnapIds { get; set; } = new List<string>();

    public bool IsRated { get; set; }

    public TimeSpan ? WorkingTime { get; set; }
    public List<BookingTechnicianDto> Technicians { get; set; } = new();
    public List<string> HexColors { get; set; } = new List<string>();
    public List<int> ProductIds { get; set; } = new List<int>();
    private class Mapping : Profile 
    {
        public Mapping()
        {
            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.TechnicianIds,
                opt => opt.MapFrom(src => src.BookingTechnicians.Select(x => x.TechnicianId).ToList()))
                .ForMember(dest => dest.ServiceIds,
                opt => opt.MapFrom(src => src.BookingTechnicians.SelectMany(x => x.Services).Select(x => x.ServiceId).ToList()))
                 .ForMember(dest => dest.SnapIds,
                opt => opt.MapFrom(src => src.BookingSnaps.Select(x => x.SnapId).ToList()))
                 .ForMember(dest => dest.GroupdIds,
                opt => opt.MapFrom(src => src.BookingSnapGroups.Select(x => x.GroupdId).ToList()))
                .ForMember(dest => dest.HexColors,
                opt => opt.MapFrom(src => src.BookingColors.Select(x => x.HexColor).ToList()))
                .ForMember(dest => dest.ProductIds,
                opt => opt.MapFrom(src => src.BookingProducts.Select(x => x.ProductId).ToList()));
        }
    }
}


public class BookingTechnicianDto
{
    public long TechnicianId { get; set; }

    public TechnicianDto? Technician { get; set; }

    public List<ServiceDto> Services { get; set; } = [];
}

using AutoMapper;
using BuildingBlocks.ApiClients.Clients.Catalog.Services.Models;
using Order.Domain.Entities;
using Serilog;

namespace Order.Application.Features.Commissions.Models;

public class CommissionDto
{
    public int BookingId { get; set; }
    public string BookingCode { get; set; } = string.Empty;
    public DateTime BookingDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public long StoreId { get; set; }
    
    // Collection of technicians and their services
    public List<BookingTechnicianCommissionDto> BookingTechnicians { get; set; } = new();
    
    private class Mapping : Profile
    {
        public Mapping()
        {
            // Map Booking -> CommissionDto
            CreateMap<Booking, CommissionDto>()
                .ForMember(dest => dest.BookingId, opt => opt.MapFrom(src => src.Id))
                //.ForMember(dest => dest.BookingCode, opt => opt.MapFrom(src => src.Code ?? string.Empty))
                .ForMember(dest => dest.BookingDate, opt => opt.MapFrom(src => src.BookingDate))
                //.ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.PaymentDate))
                //.ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId ?? 0))
                .ForMember(dest => dest.BookingTechnicians, 
                    opt => opt.MapFrom(src => src.BookingTechnicians));

            // Map BookingTechnician -> BookingTechnicianCommissionDto
            CreateMap<BookingTechnician, BookingTechnicianCommissionDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TechnicianId, opt => opt.MapFrom(src => src.TechnicianId))
                .ForMember(dest => dest.TechnicianName, opt => opt.Ignore()) // Set from API
                .ForMember(dest => dest.TechnicianAvatar, opt => opt.Ignore()) // Set from API
                .ForMember(dest => dest.TechnicianPhone, opt => opt.Ignore()) // Set from API
                .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.Services));


            // Map BookingTechnicianService -> ServiceCommissionDto
            CreateMap<BookingTechnicianService, ServiceCommissionDto>()
                .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.ServiceId))
                .ForMember(dest => dest.ServiceName, opt => opt.Ignore()) // Set from API
               ;
        }
    }
}

public class BookingTechnicianCommissionDto
{
    public int Id { get; set; }
    public long TechnicianId { get; set; }
    public string TechnicianName { get; set; } = string.Empty;
    public string TechnicianAvatar { get; set; } = string.Empty;
    public string TechnicianPhone { get; set; } = string.Empty;
    public List<ServiceCommissionDto> Services { get; set; } = new();
    public decimal TotalCommissionForTechnician { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class ServiceCommissionDto
{
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal ServicePrice { get; set; }
    public decimal Commission { get; set; }
}

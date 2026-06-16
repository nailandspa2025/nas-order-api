using AutoMapper;
using BuildingBlocks.ApiClients.Clients.Catalog.Services.Models;
using Order.Domain.Entities;
using Order.Domain.Enums;
using Serilog;

namespace Order.Application.Features.Commissions.Models;

public class CommissionDetailDto
{
    public long BookingId { get; set; }
    public long  StoreId { get; set; }
    public DateTime BookingDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public TimeSpan BookingTime { get; set; }
    public BookingStatus Status { get; set; }
    // Thông tin service
    public int ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public decimal ServicePrice { get; set; }
    public decimal CommissionAmount { get; set; }
    // Thông tin technician (nếu cần)
    public long TechnicianId { get; set; }
    public string TechnicianName { get; set; } = string.Empty;

}
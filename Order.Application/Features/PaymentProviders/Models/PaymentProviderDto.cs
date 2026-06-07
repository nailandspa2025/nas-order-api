
using AutoMapper;
using BuildingBlocks.Persistence.Models;
using Order.Domain.Entities;

namespace Order.Application.Features.PaymentProviders.Models;
public class PaymentProviderDto : BaseAuditableDto
{
    public int Id { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public List<PaymentProviderSetting> PaymentProviderSettings { get; set; } = new List<PaymentProviderSetting>();
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<PaymentProvider, PaymentProviderDto>();
        }
    }
}
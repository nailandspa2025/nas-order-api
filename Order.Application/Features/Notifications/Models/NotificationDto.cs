using AutoMapper;
using BuildingBlocks.ApiClients.Clients.Catalog.Stores.Models;
using BuildingBlocks.ApiClients.Clients.Identity.Users.Models;
using BuildingBlocks.Persistence.Models;
using Order.Domain.Entities;
using Order.Domain.Enums;

namespace Order.Application.Features.Notifications.Models;

public class NotificationDto: BaseAuditableDto
{
    public int Id { get; set; }

    public string AccountId { get; set; } = null!;

    public string? Content { get; set; }

    public DateTime SentTime { get; set; }

    public NotificationStatus Status { get; set; }

    public string? StoreName { get; set; }
    public virtual Booking? Booking { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Notification, NotificationDto>();
        }
    }
}


using AutoMapper;
using Order.Domain.Entities;
using Order.Domain.Enums;
using BuildingBlocks.Persistence.Models;

namespace Order.Application.Features.Notifications.Models;

public class NotificationDto: BaseAuditableDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = null!;

    public string? Content { get; set; }

    public DateTime SentTime { get; set; }

    public NotificationStatus Status { get; set; }

    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Notification, NotificationDto>();
        }
    }
}


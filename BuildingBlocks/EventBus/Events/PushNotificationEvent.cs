
namespace BuildingBlocks.EventBus.Events;
public class PushNotificationEvent
{
    public string UserId { get; set; }
    public string Content { get; set;}
}
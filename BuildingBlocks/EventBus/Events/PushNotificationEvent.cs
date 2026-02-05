
namespace BuildingBlocks.EventBus.Events;
public class PushNotificationEvent
{
    public string UserId { get; set; }
    public string Content { get; set; }
    public List<string> Tokens { get; set; } = new List<string>();
    public string senderInfo { get; set; }
}
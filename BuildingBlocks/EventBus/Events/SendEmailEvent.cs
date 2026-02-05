namespace BuildingBlocks.EventBus.Events;

public class SendEmailEvent
{
	public string To { get; set; } = null!;
	public List<string>? Cc { get; set; }
	public string Subject { get; set; } = null!;
	public string Body { get; set; } = null!;
}
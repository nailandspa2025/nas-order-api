namespace Order.Domain.Entities;

public class NotificationRecipient
{
    public int Id { get; set; }

    public int NotificationId { get; set; }

    public virtual Notification Notification { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public bool IsRead { get; set; } = false;
}


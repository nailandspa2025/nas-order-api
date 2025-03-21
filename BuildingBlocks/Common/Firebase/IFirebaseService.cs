using FirebaseAdmin.Messaging;

namespace BuildingBlocks.Common.Firebase;

public interface IFirebaseService
{
    Task SendNotificationAsync(string token, string title, string body);

    Task SendMulticastAsync(MulticastMessage message);

    Task<string> SendAsync(Message message);
}

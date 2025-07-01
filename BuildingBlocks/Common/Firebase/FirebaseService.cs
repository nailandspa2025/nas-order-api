using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using YamlDotNet.Core.Tokens;

namespace BuildingBlocks.Common.Firebase;

public class FirebaseService : IFirebaseService
{
    private readonly FirebaseMessaging _messaging;

    public FirebaseService(IConfiguration configuration)
    {
        var credentialPath = configuration["FirebaseSettings:CredentialApplication"];

        if (string.IsNullOrEmpty(credentialPath))
            throw new InvalidOperationException("Firebase credential file path is missing in configuration.");

        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.FromFile(credentialPath)
            });
        }

        _messaging = FirebaseMessaging.DefaultInstance;
    }

    public async Task<string> SendAsync(Message message)
    {
        try
        {
            var response = await _messaging.SendAsync(message);
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public async Task SendMulticastAsync(MulticastMessage message)
    {
        if (message.Tokens == null || !message.Tokens.Any()) return;
        foreach (var token in message.Tokens)
        {
            try
            {
                var singleMessage = new Message
                {
                    Token = token,
                    Notification = message.Notification,
                    Data = message.Data
                };

                await _messaging.SendAsync(singleMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending to token: {token} - {ex.Message}");
            }
        }
    }
    public async Task SendNotificationAsync(string token, string title, string body)
    {
        await _messaging.SendAsync(
            new Message()
            {
                Token = token,
                Notification = new Notification()
                {
                    Body = body,
                    Title = title
                }
            });
    }
}

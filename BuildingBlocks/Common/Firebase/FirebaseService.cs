using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace BuildingBlocks.Common.Firebase;

public class FirebaseService : IFirebaseService
{
    private readonly FirebaseMessaging _messaging;

   public FirebaseService(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    {
        var credentialPath = configuration["FirebaseSettings:CredentialApplication"];

        var app = FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile(credentialPath)
                .CreateScoped("https://www.googleapis.com/auth/firebase.messaging")
        });
        _messaging = FirebaseMessaging.GetMessaging(app);
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
        var tokens = new List<string>(message.Tokens);
        if (tokens != null && tokens.Any())
        {
            int skip = 0;
            int take = 500;
            do
            {
                message.Tokens = tokens.Skip(skip).Take(take).ToList();
                await _messaging.SendMulticastAsync(message);
                skip += take;
            }
            while (skip < tokens.Count());
        }
    }

    public async Task SendNotification(string token, string title, string body)
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

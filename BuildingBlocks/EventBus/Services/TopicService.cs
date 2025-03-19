using MassTransit;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.EventBus.Services;

public class TopicService : ITopicService
{
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly ILogger<TopicService> _logger;

    public TopicService(
        ISendEndpointProvider sendEndpointProvider,
        ILogger<TopicService> logger)
    {
        _sendEndpointProvider = sendEndpointProvider;
        _logger = logger;
    }

    public async Task<bool> PublicEventAsync<T>(T messageObject, string topicName) where T : class
    {
        try
        {
            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{topicName}"));
            await endpoint.Send(messageObject);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send message to serviceBus topic ${topicName}: {ex.Message}");
            return false;
        }
    }
}


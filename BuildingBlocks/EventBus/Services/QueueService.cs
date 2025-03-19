

using MassTransit;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.EventBus.Services;

public class QueueService : IQueueService
{
    private readonly ISendEndpointProvider _sendEndpointProvider;
    private readonly ILogger<QueueService> _logger;

    public QueueService(
        ISendEndpointProvider sendEndpointProvider,
        ILogger<QueueService> logger)
    {
        _sendEndpointProvider = sendEndpointProvider;
        _logger = logger;
    }

    public async Task<bool> SendToQueueAsync<T>(T messageObject, string queueName) where T : class
    {
        try
        {
            await (await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{queueName}"))).Send(messageObject);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send message to serviceBus queue ${queueName}: {ex.Message}");
            return false;
        }
    }
}

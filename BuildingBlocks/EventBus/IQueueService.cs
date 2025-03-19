using System;
namespace BuildingBlocks.EventBus;

public interface IQueueService
{
    Task<bool> SendToQueueAsync<T>(T messageObject, string queueName) where T : class;
}


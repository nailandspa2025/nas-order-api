namespace BuildingBlocks.EventBus
{
    public interface ITopicService
	{
        Task<bool> PublicEventAsync<T>(T messageObject, string topicName) where T : class;
    }
}


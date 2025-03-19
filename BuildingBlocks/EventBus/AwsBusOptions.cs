using System;
namespace BuildingBlocks.EventBus;

public class AwsBusOptions
{
    public bool UseAwsBus { get; set; }
    public string? Region { get; set; }
    public string? TargetEnvironment { get; set; }
    public Queue? Queue { get; set; }
    public Topic? Topic { get; set; }
}

public class Queue
{
    public string CommunicationSentSms { get; set; } = null!;

    public string CommunicationSentEmail { get; set; } = null!;
}

public class Topic
{
    public string IdentityUserLoggedOut { get; set; } = null!;
}


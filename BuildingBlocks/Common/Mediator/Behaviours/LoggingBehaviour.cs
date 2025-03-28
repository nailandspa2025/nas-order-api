using System;
using BuildingBlocks.Authentication.Abstractions;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Common.Mediator.Behaviours;

public class LoggingBehaviour<TRequest> : IRequestPreProcessor<TRequest> where TRequest : notnull
{
    private readonly ILogger _logger;
    private readonly ICurrentUser _currentUser;

    public LoggingBehaviour(ILogger<TRequest> logger, ICurrentUser currentUser)
    {
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task Process(TRequest request, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUser.UserId;
        var userName = _currentUser.UserName;

        const string prefix = nameof(LoggingBehaviour<TRequest>);

        _logger.LogInformation("[{Prefix}] Request: {Name} {@UserId} {@UserName} {@Request}",
            prefix, requestName, userId, userName, request);

        await Task.CompletedTask;
    }
}


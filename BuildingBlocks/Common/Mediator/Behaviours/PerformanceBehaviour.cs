using System.Diagnostics;
using BuildingBlocks.Authentication.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Common.Mediator.Behaviours;

public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer;
    private readonly ILogger<TRequest> _logger;
    private readonly ICurrentUser _currentUser;

    public PerformanceBehaviour(
        ILogger<TRequest> logger,
        ICurrentUser currentUser)
    {
        _timer = new Stopwatch();
        _logger = logger;
        _currentUser = currentUser;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        if (elapsedMilliseconds > 3000)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUser.UserId;
            var userName = _currentUser.UserName;

            const string prefix = nameof(PerformanceBehaviour<TRequest, TResponse>);

            _logger.LogWarning("[{Prefix}] Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) {@UserId} {@UserName} {@Request}", prefix,
                requestName, elapsedMilliseconds, userId, userName, request);
        }

        return response;
    }
}

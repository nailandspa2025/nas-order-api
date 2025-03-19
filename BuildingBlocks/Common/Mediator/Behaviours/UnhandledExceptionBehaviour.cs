using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Common.Mediator.Behaviours;

public class UnhandledExceptionBehaviour<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> where TRequest
    : IRequest<TResponse>
{
    private readonly ILogger<TRequest> _logger;

    public UnhandledExceptionBehaviour(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;

            const string prefix = nameof(UnhandledExceptionBehaviour<TRequest, TResponse>);

            _logger.LogError(ex, "[{Prefix}] Request: Unhandled Exception for Request {Name} {@Request}", prefix, requestName, request);

            throw;
        }
    }
}


using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CQRS.Application.Common.Behaviors;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private const int SlowRequestThresholdMs = 500;

    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;

    public PerformanceBehavior(ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var timer = Stopwatch.StartNew();

        try
        {
            return await next();
        }
        finally
        {
            timer.Stop();
            var elapsedMs = timer.ElapsedMilliseconds;
            var requestName = typeof(TRequest).Name;

            if (elapsedMs > SlowRequestThresholdMs)
            {
                _logger.LogWarning(
                    "Long running request {RequestName} took {ElapsedMilliseconds} ms {@Request}",
                    requestName,
                    elapsedMs,
                    request);
            }
            else
            {
                _logger.LogInformation(
                    "Request {RequestName} completed in {ElapsedMilliseconds} ms",
                    requestName,
                    elapsedMs);
            }
        }
    }
}

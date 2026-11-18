using CQRS.Infrastructure.Persistence;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CQRS.Infrastructure.Behaviors;

public class TransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly AppDbContext _context;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(
        AppDbContext context,
        ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!typeof(TRequest).Name.EndsWith("Command"))
            return await next();

        if (_context.Database.CurrentTransaction is not null)
            return await next();

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        var requestName = typeof(TRequest).Name;

        _logger.LogInformation("Begin transaction for {RequestName}", requestName);

        try
        {
            var response = await next();
            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            _logger.LogInformation("Committed transaction for {RequestName}", requestName);
            return response;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Rolled back transaction for {RequestName}", requestName);
            throw;
        }
    }
}

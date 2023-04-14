using MediatR;
using Microsoft.Extensions.Logging;

namespace Core.Application;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var commandType = request.GetType();

        var commandTypeName = commandType.FullName;

        try
        {
            _logger.LogDebug("Command started for {CommandTypeName}: {@Command}", commandTypeName, request);

            return await next();
        }
        catch (AggregateException aex)
        {
            _logger.LogError(aex.InnerException, "Error during command {CommandTypeName} execution", commandTypeName);

            if (aex.InnerException != null)
            {
                throw aex.InnerException;
            }

            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during command {CommandTypeName} execution", commandTypeName);

            throw;
        }
        finally
        {
            _logger.LogDebug("Command ended for {CommandTypeName}", commandTypeName);
        }
    }
}

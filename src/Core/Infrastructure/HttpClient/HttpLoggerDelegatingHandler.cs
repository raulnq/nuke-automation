using Serilog;

namespace Core.Infrastructure;

public class HttpLoggerDelegatingHandler : DelegatingHandler
{
    private static readonly Serilog.ILogger _logger = Log.Logger.ForContext<HttpLoggerDelegatingHandler>();

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestBody = request.Content != null ? await request.Content.ReadAsStringAsync() : "[None]";

        var logger = _logger.ForContext("request", requestBody);

        try
        {
            var response = await base.SendAsync(request, cancellationToken);

            var responseBody = response.Content != null ? await response.Content.ReadAsStringAsync() : "[None]";

            logger = logger.ForContext("response", responseBody);

            return response;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Method: {Method}, Uri: {RequestUri}, Version: {Version}", request.Method, request?.RequestUri?.ToString(), request?.Version?.ToString());

            throw;
        }
        finally
        {
            logger.Information("Method: {Method}, Uri: {RequestUri}, Version: {Version}", request.Method, request?.RequestUri?.ToString(), request?.Version?.ToString());
        }
    }
}
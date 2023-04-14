using AspNetCore.Authentication.ApiKey;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Core.Infrastructure;

public class ApiKeyProvider : IApiKeyProvider
{
    private readonly ILogger<IApiKeyProvider> _logger;
    private readonly ApiKeySettings _settings;

    public ApiKeyProvider(ILogger<IApiKeyProvider> logger, ApiKeySettings settings)
    {
        _logger = logger;
        _settings = settings;
    }

    public Task<IApiKey> ProvideAsync(string key)
    {
        try
        {
            _settings.TryGetValue(key, out var role);

            if (!string.IsNullOrEmpty(role))
            {
                return Task.FromResult<IApiKey>(new ApiKey(key, new List<Claim>() { new Claim(ClaimTypes.Role, role) }));
            }

            return Task.FromResult<IApiKey>(default);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);
            throw;
        }
    }
}

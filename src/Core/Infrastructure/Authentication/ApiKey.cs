using AspNetCore.Authentication.ApiKey;
using System.Security.Claims;

namespace Core.Infrastructure;

class ApiKey : IApiKey
{
    public ApiKey(string key, List<Claim>? claims = null)
    {
        Key = key;
        OwnerName = string.Empty;
        Claims = claims ?? new List<Claim>();
    }

    public string Key { get; }
    public string OwnerName { get; }
    public IReadOnlyCollection<Claim> Claims { get; }
}

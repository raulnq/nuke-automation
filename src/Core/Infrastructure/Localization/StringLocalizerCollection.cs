using Microsoft.Extensions.Localization;
using System.Collections.Concurrent;

namespace Core.Infrastructure;

public class StringLocalizerCollection
{
    private readonly IReadOnlyList<IStringLocalizer> _stringLocalizers;

    private readonly ConcurrentDictionary<string, IStringLocalizer> _stringLocalizerByKey = new();

    public StringLocalizerCollection(IEnumerable<IStringLocalizer> stringLocalizers) => _stringLocalizers = stringLocalizers.ToList();

    public string GetString(string key, object[]? arguments = null)
    {
        if (_stringLocalizerByKey.TryGetValue(key, out var stringLocalizer))
        {
            if (arguments == null)
            {
                return stringLocalizer[key];
            }

            return stringLocalizer[key, arguments];
        }

        stringLocalizer = GetStringLocalizer(key);

        if (stringLocalizer == null)
        {
            return key;
        }

        _stringLocalizerByKey.TryAdd(key, stringLocalizer);

        return GetString(key, arguments);
    }

    private IStringLocalizer? GetStringLocalizer(string key)
    {
        foreach (var stringLocalizer in _stringLocalizers)
        {
            if (stringLocalizer.GetString(key).ResourceNotFound)
            {
                continue;
            }

            return stringLocalizer;
        }

        return null;
    }
}
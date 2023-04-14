using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration)
    {
        var corsSection = configuration.GetSection("Cors");

        if (!corsSection.Exists())
        {
            return services;
        }

        return services.AddCors(options =>
        {
            foreach (var item in corsSection.GetChildren())
            {
                var origins = item.Get<string[]>();

                options.AddPolicy(item.Key, builder => builder.WithOrigins(origins!).AllowAnyMethod().AllowAnyHeader());
            }
        });
    }
}
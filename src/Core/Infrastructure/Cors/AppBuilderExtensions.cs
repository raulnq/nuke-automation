using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Core.Infrastructure;

public static partial class AppBuilderExtensions
{
    public static IApplicationBuilder UseCors(this IApplicationBuilder app, IConfiguration configuration)
    {
        var infrastructureConfiguration = configuration.GetSection("Infrastructure");

        var corsSection = infrastructureConfiguration.GetSection("Cors");

        if (corsSection == null)
        {
            return app;
        }

        foreach (var item in corsSection.GetChildren())
        {
            app.UseCors(item.Key);
        }

        return app;
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace Core.Infrastructure;

public static partial class AppBuilderExtensions
{
    public static IApplicationBuilder UseLocalization(this IApplicationBuilder app)
    {
        var supportedCultures = new[]
        {
                new CultureInfo("en-US"),
                new CultureInfo("es")
            };

        return app
                .UseRequestLocalization(new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture("en-US"),
                    SupportedCultures = supportedCultures,
                    SupportedUICultures = supportedCultures
                });
    }
}

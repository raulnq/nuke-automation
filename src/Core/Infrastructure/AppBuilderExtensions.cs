using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Builder;

namespace Core.Infrastructure;

public static partial class AppBuilderExtensions
{
    public static IApplicationBuilder UseProblemDetails(this IApplicationBuilder app) =>
        ProblemDetailsExtensions.UseProblemDetails(app);

    public static IApplicationBuilder UseHealthChecks(this IApplicationBuilder app) => app.UseHealthChecks("/health");
}
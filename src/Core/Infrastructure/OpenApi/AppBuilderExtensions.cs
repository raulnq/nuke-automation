using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Core.Infrastructure;

public static partial class AppBuilderExtensions
{
    public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, IConfiguration configuration)
    {
        var infrastructureConfiguration = configuration.GetSection("Infrastructure");

        var config = infrastructureConfiguration.GetSection("Swagger");

        if (!config.Exists())
        {
            return app;
        }

        app.UseSwagger();

        app.UseSwaggerUI(c => c.SwaggerEndpoint(config.GetValue("Url", "/swagger/v1/swagger.json"), config.GetValue("Title", "None")));

        return app;
    }
}

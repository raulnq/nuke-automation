using Hellang.Middleware.ProblemDetails;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Core.Infrastructure;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
    {
        services.AddOptions();

        services.AddHttpContextAccessor();

        services.AddHealthChecks();

        services.AddProblemDetails(options => ConfigureProblemDetails(options));

        services.AddAuthentication(configuration);

        services.AddCors(configuration);

        services.AddSwagger(configuration);

        services.AddLoggy(configuration);

        services.AddMemoryCache();

        services.AddQueryRunners(configuration, assembly);

        services.AddRebus(configuration, assembly);

        services.AddLocalization(assembly);

        return services;
    }
}

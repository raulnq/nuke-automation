using Core.Application;

namespace WebAPI.Application;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddApplication(typeof(ServiceCollectionExtensions).Assembly);

        return services;
    }
}
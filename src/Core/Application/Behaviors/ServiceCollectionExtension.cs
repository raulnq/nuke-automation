using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application;

public static partial class ServiceCollectionExtension
{
    public static IServiceCollection AddBehaviors(this IServiceCollection services)
    {
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(PublishDomainEventBehavior<,>));

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        return services;
    }
}
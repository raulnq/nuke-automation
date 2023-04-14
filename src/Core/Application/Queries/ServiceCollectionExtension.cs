using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace Core.Application;

public static partial class ServiceCollectionExtension
{
    public static IServiceCollection AddQueryHandler<TQuery, TResult>(this IServiceCollection services)
                where TQuery : BaseQuery<TResult>, IRequest<TResult>
    {
        return services.AddTransient<IRequestHandler<TQuery, TResult>, QueryHandler<TQuery, TResult>>();
    }

    public static IServiceCollection AddQueryHandlers(this IServiceCollection services, params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            var types = assembly.DefinedTypes.Where(type => typeof(IQuery).IsAssignableFrom(type) && type.AsType() != typeof(IQuery));

            foreach (var queryInfoType in types)
            {
                var requestInterfaceType = queryInfoType.ImplementedInterfaces.First(t => t != typeof(IQuery));

                var resultType = requestInterfaceType.GenericTypeArguments.First();

                var genericRequestHandlerType = typeof(IRequestHandler<,>);

                var queryType = queryInfoType.AsType();

                var requestHandlerType = genericRequestHandlerType.MakeGenericType(queryType, resultType);

                var genericQueryHandlerType = typeof(QueryHandler<,>);

                var queryHandlerType = genericQueryHandlerType.MakeGenericType(queryType, resultType);

                var descriptor = ServiceDescriptor.Describe(requestHandlerType, queryHandlerType, ServiceLifetime.Transient);

                services.Add(descriptor);
            }
        }

        return services;
    }
}
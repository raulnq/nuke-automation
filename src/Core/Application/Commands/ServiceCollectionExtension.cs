using AutoMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Core.Domain;
using Core.Events;

namespace Core.Application;

public static partial class ServiceCollectionExtension
{
    public static IServiceCollection AddEventDispatcher<TDomainEvent, TEvent>(this IServiceCollection services,
        Action<IMappingExpression<TDomainEvent, TEvent>>? mapper = null,
        Func<TDomainEvent, bool>? when = null, Action<MapperConfigurationExpression>? mapperOptions = null)
        where TDomainEvent : IDomainEvent
        where TEvent : IEvent
    {
        if (mapper != null)
        {
            services.PostConfigure<MapperConfigurationExpression>((options) =>
            {
                mapperOptions?.Invoke(options);

                mapper(options.CreateMap<TDomainEvent, TEvent>());
            });
        }
        else
        {
            services.PostConfigure<MapperConfigurationExpression>((options) =>
            {
                mapperOptions?.Invoke(options);

                options.CreateMap<TDomainEvent, TEvent>();
            });
        }

        return services.AddTransient<INotificationHandler<TDomainEvent>, EventDispatcher<TDomainEvent, TEvent>>(provider => new EventDispatcher<TDomainEvent, TEvent>(provider.GetRequiredService<IEventPublisher>(), provider.GetRequiredService<IMapper>(), when));
    }

    public static IServiceCollection AddBaseEventDispatcher<TDomainEvent, TEventDispacher>(this IServiceCollection services)
        where TDomainEvent : IDomainEvent
        where TEventDispacher : BaseEventDispatcher<TDomainEvent>
    {
        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(INotificationHandler<TDomainEvent>) && s.ImplementationType == typeof(TEventDispacher));

        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        services.AddTransient<INotificationHandler<TDomainEvent>, TEventDispacher>();

        return services;
    }

    public static IServiceCollection AddCommandDispatcher<TDomainEvent, TCommand>(this IServiceCollection services,
        Action<IMappingExpression<TDomainEvent, TCommand>>? mapper = null,
        Func<TDomainEvent, bool>? when = null, Action<MapperConfigurationExpression>? mapperOptions = null)
        where TDomainEvent : IDomainEvent
        where TCommand : ICommand
    {
        if (mapper != null)
        {
            services.PostConfigure<MapperConfigurationExpression>((options) =>
            {
                mapperOptions?.Invoke(options);

                mapper(options.CreateMap<TDomainEvent, TCommand>());
            });
        }
        else
        {
            services.PostConfigure<MapperConfigurationExpression>((options) =>
            {
                mapperOptions?.Invoke(options);

                options.CreateMap<TDomainEvent, TCommand>();
            });
        }
        return services.AddTransient<INotificationHandler<TDomainEvent>, CommandDispatcher<TDomainEvent, TCommand>>(provider => new CommandDispatcher<TDomainEvent, TCommand>(provider.GetRequiredService<IMediator>(), provider.GetRequiredService<IMapper>(), when));
    }

    public static void AddBaseCommandDispatcher<TDomainEvent, TCommandDispacher>(this IServiceCollection services)
        where TDomainEvent : IDomainEvent
        where TCommandDispacher : BaseCommandDispatcher<TDomainEvent>
    {
        var descriptor = services.FirstOrDefault(s => s.ServiceType == typeof(INotificationHandler<TDomainEvent>) && s.ImplementationType == typeof(TCommandDispacher));

        if (descriptor != null)
        {
            services.Remove(descriptor);
        }

        services.AddTransient<INotificationHandler<TDomainEvent>, TCommandDispacher>();
    }
}

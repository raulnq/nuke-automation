using AutoMapper;
using MediatR;
using Rebus.Bus;
using Rebus.Config;
using Rebus.Handlers;
using Rebus.Pipeline;
using Rebus.Pipeline.Receive;
using Rebus.Serialization.Json;
using Serilog;
using System.Reflection;
using Core.Application;
using Core.Events;
using static Rebus.Routing.TypeBased.TypeBasedRouterConfigurationExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Core.Infrastructure;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommandDispatcher<TEvent, TCommand>(this IServiceCollection services, Action<IMappingExpression<TEvent, TCommand>>? mapper = null,
        Func<TEvent, bool>? when = null, Action<MapperConfigurationExpression>? mapperOptions = null)
        where TEvent : IEvent
        where TCommand : ICommand
    {
        if (mapper != null)
        {
            services.PostConfigure<MapperConfigurationExpression>((options) =>
            {
                mapperOptions?.Invoke(options);

                mapper(options.CreateMap<TEvent, TCommand>());
            });
        }
        else
        {
            services.PostConfigure<MapperConfigurationExpression>((options) =>
            {
                mapperOptions?.Invoke(options);

                options.CreateMap<TEvent, TCommand>();
            });
        }

        return services.AddTransient<IHandleMessages<TEvent>, CommandDispatcher<TEvent, TCommand>>(provider => new CommandDispatcher<TEvent, TCommand>(provider.GetRequiredService<IMediator>(), provider.GetRequiredService<IMapper>(), when));
    }

    public static IServiceCollection AddCommandDispatcher<TCommand>(this IServiceCollection services)
        where TCommand : ICommand
    {
        return services.AddTransient<IHandleMessages<TCommand>, CommandDispatcher<TCommand>>();
    }

    public static IServiceCollection AddRebus(this IServiceCollection services, IConfiguration configuration, Assembly assembly, Action<IConfiguration, TypeBasedRouterConfigurationBuilder>? map = null, Func<IBus, Task>? onCreated = null)
    {
        var rebusConfig = configuration.GetSection("Rebus");

        if (!rebusConfig.Exists())
        {
            return services;
        }

        var rabbitmqConfig = configuration.GetSection("RabbitMQ");

        var serviceBusConfig = configuration.GetSection("AzureServiceBus");

        if (rabbitmqConfig.Exists() || serviceBusConfig.Exists())
        {
            var queue = rebusConfig.GetValue("Queue", "default");

            services.AutoRegisterHandlersFromAssembly(assembly);

            services.AddSingleton<IEventPublisher, RebusEventPublisher>();

            services.AddSingleton<ICommandSender, RebusCommandSender>();

            var logger = Log.ForContext("queue", queue);

            services
                .AddRebus(configurer =>
                {
                    return configurer
                        .Logging(l => l.Serilog(logger))
                        .Serialization(s => s.UseNewtonsoftJson(JsonInteroperabilityMode.PureJson))
                        .Transport(t =>
                        {
                            if (serviceBusConfig.Exists())
                            {
                                var serviceBusConnectionString = serviceBusConfig["ConnectionString"];

                                var automaticallyRenewPeekLock = serviceBusConfig.GetValue("AutomaticallyRenewPeekLock", false);

                                if (automaticallyRenewPeekLock)
                                {
                                    t.UseAzureServiceBus(serviceBusConnectionString, queue).AutomaticallyRenewPeekLock();
                                }
                                else
                                {
                                    t.UseAzureServiceBus(serviceBusConnectionString, queue);
                                }
                            }
                            else
                            {
                                if (rabbitmqConfig.Exists())
                                {
                                    var rabbitMqConnectionString = rabbitmqConfig["ConnectionString"];

                                    t.UseRabbitMq(rabbitMqConnectionString, queue);
                                }
                            }
                        })
                        .Routing(r =>
                        {
                            if (map != null)
                            {
                                map(configuration, r.TypeBased());
                            }
                            else
                            {
                                r.TypeBased();
                            }
                        })
                        .Options(o =>
                        {
                            o.Decorate<IPipeline>(c =>
                            {
                                var pipeline = c.Get<IPipeline>();
                                var stepToInject = new MessageTracingStep();

                                return new PipelineStepInjector(pipeline)
                                    .OnReceive(stepToInject, PipelineRelativePosition.Before, typeof(DispatchIncomingMessageStep));
                            });
                        });
                }, onCreated: onCreated);
        }



        return services;
    }
}
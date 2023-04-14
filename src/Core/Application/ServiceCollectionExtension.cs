using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Core.Domain;

namespace Core.Application;

public static partial class ServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddBehaviors();

        services.AddSingleton<IClock>(new SystemClock());

        services.AddAutoMapper(assemblies);

        services.AddValidatorsFromAssemblies(assemblies);

        services.AddMediatR(assemblies);

        services.AddQueryHandlers(assemblies);

        return services;
    }
}
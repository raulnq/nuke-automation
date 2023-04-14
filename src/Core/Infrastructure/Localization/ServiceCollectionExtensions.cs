using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Reflection;
using Core.Application;
using Core.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace Core.Infrastructure;

public static partial class ServiceCollectionExtensions
{
    private static IServiceCollection AddLocalization(this IServiceCollection services, Assembly assembly)
    {
        services.AddLocalization(options => options.ResourcesPath = "");

        var resourceTypes = Assembly.Load(assembly.GetName())
                               .DefinedTypes
                               .Where(type => typeof(ILocalizationResource).IsAssignableFrom(type) && type.AsType() != typeof(ILocalizationResource));

        services.AddSingleton(serviceProvider =>
        {
            var factory = serviceProvider.GetRequiredService<IStringLocalizerFactory>();

            var stringLocalizers = new List<IStringLocalizer>();

            if (resourceTypes is not null)
            {
                foreach (var resourceType in resourceTypes)
                {
                    if (resourceType is not null)
                    {
                        var stringLocalizer = factory.Create(resourceType.GetTypeInfo().AsType());

                        stringLocalizers.Add(stringLocalizer);
                    }
                }
            }

            return new StringLocalizerCollection(stringLocalizers);
        });

        return services;
    }


    private static void ConfigureProblemDetails(Hellang.Middleware.ProblemDetails.ProblemDetailsOptions options)
    {
        options.IncludeExceptionDetails = (_, _) => false;

        options.Rethrow<NotSupportedException>();

        options.Map<NotFoundException>((httpContext, exception) =>
        {
            return new ProblemDetails()
            {
                Type = "not-found-error",
                Title = "The specified resource was not found.",
                Detail = GetDetail(httpContext.RequestServices, exception),
                Status = StatusCodes.Status404NotFound
            };
        });

        options.Map<ValidationException>((httpContext, exception) =>
        {
            var stringLocalizerCollection = httpContext.RequestServices.GetRequiredService<StringLocalizerCollection>();

            var errors = exception.Errors.ToDictionary(
                            error => error.Key,
                            error => error.Value
                                        .Select(validationError =>
                                            stringLocalizerCollection.GetString(validationError.Code, validationError.Parameters))
                                        .ToArray());

            return new ValidationProblemDetails(errors)
            {
                Title = "One or more validation errors occurred.",
                Detail = GetDetail(httpContext.RequestServices, exception),
                Type = "validation-error",
                Status = StatusCodes.Status400BadRequest
            };
        });

        options.Map<DomainException>((httpContext, exception) =>
        {
            return new ProblemDetails()
            {
                Title = "A domain error has ocurred.",
                Detail = GetDetail(httpContext.RequestServices, exception),
                Type = "domain-error",
                Status = StatusCodes.Status400BadRequest
            };
        });

        options.Map<Application.ApplicationException>((httpContext, exception) =>
        {
            return new ProblemDetails()
            {
                Title = "An application error has ocurred.",
                Detail = GetDetail(httpContext.RequestServices, exception),
                Type = "application-error",
                Status = StatusCodes.Status400BadRequest
            };
        });

        options.Map<InfrastructureException>((httpContext, exception) =>
        {
            return new ProblemDetails()
            {
                Title = "An infrastructure error has ocurred.",
                Detail = GetDetail(httpContext.RequestServices, exception),
                Type = "infrastructure-error",
                Status = StatusCodes.Status400BadRequest
            };
        });

        options.MapStatusCode = (httpContext) =>
        {
            var statusCode = httpContext.Response.StatusCode;

            var stringLocalizerCollection = httpContext.RequestServices.GetRequiredService<StringLocalizerCollection>();

            if (statusCode == StatusCodes.Status401Unauthorized)
            {
                return new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = stringLocalizerCollection.GetString("Unauthorized"),
                    Title = "Security error",
                    Type = "unauthorized",
                    Instance = httpContext.Request.Path
                };
            }

            if (statusCode == StatusCodes.Status403Forbidden)
            {
                return new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Detail = stringLocalizerCollection.GetString("Forbidden"),
                    Title = "Security error",
                    Type = "forbidden",
                    Instance = httpContext.Request.Path
                };
            }

            return new StatusCodeProblemDetails(statusCode);
        };
    }

    private static string GetDetail(IServiceProvider serviceProvider, BaseException exception)
    {
        if (string.IsNullOrEmpty(exception.Description))
        {
            var stringLocalizerCollection = serviceProvider.GetRequiredService<StringLocalizerCollection>();

            return stringLocalizerCollection.GetString(exception.Code, exception.Parameters);
        }

        return exception.Description;
    }
}

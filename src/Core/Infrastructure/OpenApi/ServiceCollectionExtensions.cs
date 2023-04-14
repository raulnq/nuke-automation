using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Core.Infrastructure;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        var config = configuration.GetSection("Swagger");

        if (!config.Exists())
        {
            return services;
        }

        var apiKeysConfig = configuration.GetSection("ApiKeys");

        var apiKeysConfigEnabled = apiKeysConfig.Exists();

        var jwtConfig = configuration.GetSection("Jwt");

        var jwtConfigEnabled = jwtConfig.Exists();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.DescribeAllParametersInCamelCase();

            options.CustomSchemaIds(CustomSchemaIdProvider.Get);

            options.SwaggerDoc("v1", new OpenApiInfo { Title = config.GetValue("Title", "None"), Version = config.GetValue("Version", "1.0.0") });

            if (apiKeysConfigEnabled)
            {
                options.AddSecurityDefinition(KeyName, new OpenApiSecurityScheme
                {
                    Description = "API Key Authorization header. Example \"{token}\"",
                    In = ParameterLocation.Header,
                    Name = KeyName,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = ApiKeyDefaults.AuthenticationScheme

                });
            }

            if (jwtConfigEnabled)
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });
            }

            var requirements = new OpenApiSecurityRequirement();

            if (apiKeysConfigEnabled)
            {
                requirements.Add(new OpenApiSecurityScheme
                {
                    Name = KeyName,
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = KeyName
                    },
                }, new List<string>());
            }

            if (jwtConfigEnabled)
            {
                requirements.Add(new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = JwtBearerDefaults.AuthenticationScheme
                    }
                }, new List<string>());
            }

            if (requirements.Any())
            {
                options.AddSecurityRequirement(requirements);
            }
        });

        return services;
    }
}
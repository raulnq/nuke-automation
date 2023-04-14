using AspNetCore.Authentication.ApiKey;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Core.Infrastructure;

public static partial class ServiceCollectionExtensions
{
    public const string KeyName = "X-API-KEY";

    private static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration, Func<AuthenticationBuilder, AuthenticationBuilder> authenticationBuilder = null)
    {
        var jwtConfig = configuration.GetSection("Jwt");

        var apiConfig = configuration.GetSection("ApiKeys");

        var jwtEnabled = jwtConfig.Exists();

        var apiEnabled = apiConfig.Exists();

        if (!jwtEnabled && !apiEnabled)
        {
            return services;
        }

        var builder = services.AddAuthentication(options =>
        {
            if (jwtEnabled)
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
        });

        if (jwtEnabled)
        {
            var jwtSettings = jwtConfig.Get<JwtSettings>()!;

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = true,
                ValidateLifetime = true
            };

            builder.AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtSettings.Issuer;
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });
        }

        if (apiEnabled)
        {
            var apiSettings = apiConfig.Get<ApiKeySettings>()!;

            services.AddSingleton(apiSettings);

            builder.AddApiKeyInHeaderOrQueryParams<ApiKeyProvider>(ApiKeyDefaults.AuthenticationScheme, options =>
            {
                options.KeyName = KeyName;
                options.SuppressWWWAuthenticateHeader = true;
            });
        }

        services.AddAuthorization();

        return services;
    }
}
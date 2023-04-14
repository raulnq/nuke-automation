using Core.Domain;
using Core.Infrastructure;

namespace WebAPI.Infrastructure;

public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var infrastructureConfiguration = configuration.GetSection("Infrastructure");

        services.AddInfrastructure(infrastructureConfiguration, typeof(ServiceCollectionExtensions).Assembly);

        services.AddPersistance<ApplicationDbContext>(infrastructureConfiguration);

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        return services;
    }
}
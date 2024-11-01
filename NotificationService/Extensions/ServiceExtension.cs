using NotificationService.Filters;
using NotificationService.Mapper;
using NotificationService.Repository.Contract;
using NotificationService.Repository.Implementation;

namespace NotificationService.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        // Scoped services registration
        services.AddScoped<ValidationFilterAttribute>();

        // Mapper-related scoped services
        services.AddScoped<IMapperManager, MapperManager>();

        // Repository-related scoped services
        services.AddScoped<IRepositoryManager, RepositoryManager>();

        return services;
    }
}

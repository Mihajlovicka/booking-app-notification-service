using NotificationService.Data;
using NotificationService.Filters;
using NotificationService.Repository.Contract;
using NotificationService.Repository.Implementation;
using NotificationService.Service.Contract;
using NotificationService.Service.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SignalR;

namespace NotificationService.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddCustomServices(this IServiceCollection services)
    {
        services.AddSignalR(options =>
        {
            // optional hub options
        });

        services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

        
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddScoped<ValidationFilterAttribute>();

        services.AddSingleton<AppDbContext>();
        // Repository-related scoped services
        services.AddScoped<IRepositoryManager, RepositoryManager>();
        services.AddScoped(typeof(ICrudRepository<>), typeof(CrudRepository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationTypeRepository, NotificationTypeRepository>();
        services.AddScoped<INotificationUserRepository, NotificationUserRepository>();


        services.AddScoped<INotificationService, Service.Implementation.NotificationService>();

        services.AddScoped<MongoMigrationRunner>();


        services.AddScoped<NotificationSeeder>();



        return services;
    }
}

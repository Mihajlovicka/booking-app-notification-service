using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using NotificationService.Service.MessagingService;
using NotificationService.WebSocket;

namespace NotificationService.Extensions;

public static class KafkaExtensions
{
    public static IServiceCollection AddKafkaServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Configure Producer
        services.Configure<ProducerConfig>(configuration.GetSection("KafkaConfig:Producer"));
        services.AddSingleton<ProducerService>();

        // Configure Consumer
        services.Configure<ConsumerConfig>(configuration.GetSection("KafkaConfig:Consumer"));
        services.AddHostedService(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<ConsumerService>>();
            var consumerConfig = provider.GetRequiredService<IOptions<ConsumerConfig>>();
            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();
            var hubContext = provider.GetRequiredService<IHubContext<NotificationHub>>();
            var topics = new[]
            {
                KafkaTopic.DeleteUser,
                KafkaTopic.UserCreated,
                KafkaTopic.NotificationCreated
            };

            return new ConsumerService(logger, consumerConfig, topics, scopeFactory, hubContext);
        });

        return services;
    }
}

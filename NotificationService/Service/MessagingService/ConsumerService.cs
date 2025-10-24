using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NotificationService.Model.Dto;
using NotificationService.Model.Entity;
using NotificationService.Model.Messages;
using NotificationService.Repository.Contract;
using Microsoft.AspNetCore.SignalR;
using NotificationService.WebSocket;

namespace NotificationService.Service.MessagingService;

public class ConsumerService : BackgroundService
{
    private readonly ILogger<ConsumerService> _logger;
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IEnumerable<KafkaTopic> _topicNames;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<NotificationHub> _hubContext;

    public ConsumerService(
        ILogger<ConsumerService> logger,
        IOptions<ConsumerConfig> config,
        IEnumerable<KafkaTopic> topicNames,
        IServiceScopeFactory scopeFactory,
        IHubContext<NotificationHub> hubContext
    )
    {
        _logger = logger;
        _topicNames = topicNames;
        _scopeFactory = scopeFactory;
         _hubContext = hubContext;
        _consumer = new ConsumerBuilder<Ignore, string>(config.Value).Build();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
         _consumer.Subscribe(_topicNames.Select(t => t.ToString()).ToList());

        Task.Run(
            async () =>
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        using var scope = _scopeFactory.CreateScope();

                        var repositoryManager = scope.ServiceProvider.GetRequiredService<IRepositoryManager>();

                        var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(5));
                        if (consumeResult is null)
                            continue;

                        var topic = consumeResult.Topic;
                        _logger.LogInformation($"Kafka message received on topic {topic}: {consumeResult.Message.Value}");
                        
                        var topicEnum = Enum.Parse<KafkaTopic>(topic);

                        var type = TopicTypeMap.Map.GetValueOrDefault(topicEnum)
                               ?? throw new InvalidOperationException($"No type map found for topic {topic}");

                        var message = JsonConvert.DeserializeObject(consumeResult.Message.Value, type);

                        switch (message)
                        {
                            case NotificationDto notificationDto:
                                var user = await repositoryManager.UserRepository.GetByExternalIdAsync(notificationDto.NotificationUserExternalId);
                                if(user != null)
                                {
                                    var existing = await repositoryManager.NotificationUserRepository
                                            .GetByUserAndTypeAsync(user.ExternalId.ToString(), notificationDto.NotificationTypeId);

                                    if (existing != null)
                                    {
                                        var notification = new Notification(notificationDto)
                                        {
                                            NotificationUserExternalId = user.ExternalId.ToString()
                                        };
                                        
                                        await repositoryManager.NotificationRepository.AddAsync(notification);

                                        await _hubContext.Clients.User(user.Username).SendAsync("ReceiveNotification", new
                                        {
                                            Message = notificationDto.Message,
                                            Seen = false,
                                            NotificationUserExternalId = notificationDto.NotificationUserExternalId,
                                            NotificationTypeId = notificationDto.NotificationTypeId
                                        });

                                        _logger.LogInformation($"Notification sent to user {user.Username} via WebSocket.");
                                    }
                                }
                                break;
                            case UserDto userDto:

                                if (topic == KafkaTopic.UserCreated.ToString())
                                {
                                    var userCreated = new User(userDto);
                                    await repositoryManager.UserRepository.AddAsync(userCreated);

                                    List<NotificationType> notificationTypes = await repositoryManager.NotificationTypeRepository.GetByRoleAsync(userCreated.Role);

                                    foreach(NotificationType notificationType in notificationTypes)
                                    {
                                        var notificationUser = new NotificationUser
                                        {
                                            NotificationTypeId = notificationType.Id,
                                            UserExternalId = userCreated.ExternalId.ToString()
                                        };
                                        await repositoryManager.NotificationUserRepository.AddAsync(notificationUser);
                                    }

                                    _logger.LogInformation($"User '{userDto.Username}' saved in NotificationService.");
                                }
                                if(topic == KafkaTopic.DeleteUser.ToString())
                                {
                                    var userDeleted = await repositoryManager.UserRepository.GetByExternalIdAsync(userDto.Id);
                                    await repositoryManager.UserRepository.DeleteAllForUserAsync(userDeleted.Id);
                                    _logger.LogInformation($"User '{userDto.Username}' deleted in NotificationService.");
                                }
                                break;
                            default:
                                _logger.LogWarning($"Unknown message type received for topic {topic}");
                                break;
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Ignore
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error consuming message: {ex.Message}");
                    }
                }
            },
            stoppingToken
        );

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}

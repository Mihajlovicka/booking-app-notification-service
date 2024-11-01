namespace NotificationService.Service.MessagingService;

public static class TopicTypeMap
{
    public static readonly Dictionary<KafkaTopic, Type> Map =
        new() { { KafkaTopic.TestTopic, typeof(Object) } };
}

public enum KafkaTopic
{
    TestTopic,
    AnotherTopic,
}

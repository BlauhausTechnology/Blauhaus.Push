namespace Blauhaus.Push.Server._Config
{
    public interface IPushNotificationsServerConfig
    {
        string NotificationHubName { get; }
        string ConnectionString { get; }
    }
}
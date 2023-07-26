namespace Blauhaus.Push.Abstractions.Client
{
    public interface IPushNotificationsClientConfig
    {
        string NotificationHubName { get; }
        string ConnectionString { get; }
    }
}
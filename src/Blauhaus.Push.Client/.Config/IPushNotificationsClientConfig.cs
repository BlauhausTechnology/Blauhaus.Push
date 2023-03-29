namespace Blauhaus.Push.Client.Config
{
    public interface IPushNotificationsClientConfig
    {
        string NotificationHubName { get; }
        string ConnectionString { get; }
    }
}
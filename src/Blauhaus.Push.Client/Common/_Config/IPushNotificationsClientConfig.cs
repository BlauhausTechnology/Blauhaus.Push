namespace Blauhaus.Push.Client.Common._Config
{
    public interface IPushNotificationsClientConfig
    {
        string NotificationHubName { get; }
        string ConnectionString { get; }
    }
}
namespace Blauhaus.Push.Client.Common.Config
{
    public interface IPushNotificationsClientConfig
    {
        string NotificationHubName { get; }
        string ConnectionString { get; }
    }
}
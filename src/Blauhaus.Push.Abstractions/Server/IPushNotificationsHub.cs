namespace Blauhaus.Push.Abstractions.Server
{
    public interface IPushNotificationsHub
    {
        string NotificationHubName { get; }
        string NotificationHubConnectionString { get; }
    }
}
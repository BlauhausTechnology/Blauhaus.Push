namespace Blauhaus.Push.Abstractions.Server
{
    public class PushNotificationsHub : IPushNotificationsHub
    {
        public PushNotificationsHub(string notificationHubName, string notificationHubConnectionString)
        {
            NotificationHubName = notificationHubName;
            NotificationHubConnectionString = notificationHubConnectionString;
        }

        public string NotificationHubName { get; }
        public string NotificationHubConnectionString { get; }
    }
}
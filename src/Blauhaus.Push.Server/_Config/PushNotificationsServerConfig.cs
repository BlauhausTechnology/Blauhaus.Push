namespace Blauhaus.Push.Server._Config
{
    public class PushNotificationsServerConfig : IPushNotificationsServerConfig
    {
        public PushNotificationsServerConfig(string notificationHubName, string connectionString)
        {
            NotificationHubName = notificationHubName;
            NotificationHubConnectionString = connectionString;
        }

        public string NotificationHubName { get; }
        public string NotificationHubConnectionString { get; }
    }
}
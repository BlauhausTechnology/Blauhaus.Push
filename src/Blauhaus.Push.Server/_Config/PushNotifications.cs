using Blauhaus.Push.Abstractions.Server;

namespace Blauhaus.Push.Server._Config
{
    public class PushNotifications : IPushNotificationsHub
    {
        public PushNotifications(string notificationHubName, string connectionString)
        {
            NotificationHubName = notificationHubName;
            NotificationHubConnectionString = connectionString;
        }

        public string NotificationHubName { get; }
        public string NotificationHubConnectionString { get; }
    }
}
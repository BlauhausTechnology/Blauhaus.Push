namespace Blauhaus.Push.Client.Config
{
    public class PushNotificationsClientConfig : IPushNotificationsClientConfig
    {
        public string NotificationHubName { get; set; }
        public string ConnectionString { get; set; }
    }
}
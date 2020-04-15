namespace Blauhaus.Push.Client.Common._Config
{
    public class PushNotificationsClientConfig : IPushNotificationsClientConfig
    {
        public string NotificationHubName { get; set; }
        public string ConnectionString { get; set; }
    }
}
namespace Blauhaus.Push.Client.Common.Config
{
    public class PushNotificationsClientConfig : IPushNotificationsClientConfig
    {
        public string NotificationHubName { get; set; }
        public string ConnectionString { get; set; }
    }
}
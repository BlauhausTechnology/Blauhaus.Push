using Blauhaus.Push.Server._Config;

namespace Blauhaus.Push.Runner.Config
{
    public class DevAdmin : IPushNotificationsServerConfig
    {
        public DevAdmin()
        {
            NotificationHubConnectionString =
                "Endpoint=sb://minegamedevadmin.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=6ULhJh6mDzlRr6r0dZ2T6dL8MZHHve10nMq2V36c2T0=;" +
                "EntityPath=minegamedevadmin";

            NotificationHubName = "minegamedevadmin";
        }

        public string NotificationHubName { get; }
        public string NotificationHubConnectionString { get; }
    }
}
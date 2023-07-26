using Blauhaus.Common.Configuration.Extensions;
using Blauhaus.Push.Abstractions.Client;
using Microsoft.Extensions.Configuration;

namespace Blauhaus.Push.Client.Config
{
    public class ConfigurationPushNotificationsClientConfig : IPushNotificationsClientConfig
    {

        public ConfigurationPushNotificationsClientConfig(IConfiguration configuration)
        {
            NotificationHubName = configuration.GetRequiredString("PushNotifications", nameof(NotificationHubName));
            ConnectionString = configuration.GetRequiredString("PushNotifications", nameof(ConnectionString));
            
        }

        public string NotificationHubName { get; }
        public string ConnectionString { get; }
    }
}
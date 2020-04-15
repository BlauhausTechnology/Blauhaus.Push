using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Server._Config;

namespace Blauhaus.Push.Runner.Config
{
    public class PushRunnerConfig : IPushNotificationsServerConfig
    {
        public string NotificationHubName { get; set; }
        public string NotificationHubConnectionString { get; set;}
        public string PnsHandle { get; set;}
        public string DeviceId { get; set;}
        public IRuntimePlatform Platform { get; set; }
    }
}
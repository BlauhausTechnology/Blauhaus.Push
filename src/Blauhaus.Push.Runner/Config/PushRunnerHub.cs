using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Server._Config;

namespace Blauhaus.Push.Runner.Config
{
    public abstract class BasePushRunnerHub : IPushNotificationsHub
    {
        protected BasePushRunnerHub(IRuntimePlatform platform, string pnsHandle, string deviceId, string userId)
        {
            DeviceId = deviceId =="" ? platform.Value + "_deviceId" : deviceId;
            Platform = platform;
            PnsHandle = pnsHandle;
            UserId = userId;
        }

        public string NotificationHubName { get; set; }
        public string NotificationHubConnectionString { get; set;}
        public string PnsHandle { get; set;}
        public string DeviceId { get; set;}
        public string UserId { get; set;}
        public IRuntimePlatform Platform { get; set; }
    }
}
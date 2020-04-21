using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Server._Config;
using Blauhaus.Push.Server.Service;

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

            if((RuntimePlatform) platform == RuntimePlatform.iOS)
                DeviceTarget = Server.Service.DeviceTarget.iOS(pnsHandle);
            else if((RuntimePlatform) platform == RuntimePlatform.UWP)
                DeviceTarget = Server.Service.DeviceTarget.UWP(pnsHandle);
            else if((RuntimePlatform) platform == RuntimePlatform.Android)
                DeviceTarget = Server.Service.DeviceTarget.Android(pnsHandle);
        }

        public string NotificationHubName { get; set; }
        public string NotificationHubConnectionString { get; set;}
        public string PnsHandle { get; set;}
        public string DeviceId { get; set;}
        public string UserId { get; set;}
        public IRuntimePlatform Platform { get; set; }
        public IDeviceTarget DeviceTarget { get; }
    }
}
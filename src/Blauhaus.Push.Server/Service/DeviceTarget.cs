using System;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions.Server;

namespace Blauhaus.Push.Server.Service
{
    public class DeviceTarget : IDeviceTarget
    {
        private DeviceTarget(IRuntimePlatform platform, string pushNotificationServicesHandle)
        {
            if (string.IsNullOrWhiteSpace(pushNotificationServicesHandle))
            {
                throw new ArgumentException("PushNotificationServicesHandle is required");
            }
            Platform = platform;
            PushNotificationServicesHandle = pushNotificationServicesHandle;
        }

        public IRuntimePlatform Platform { get; }
        public string PushNotificationServicesHandle { get; }

        public static DeviceTarget iOS(string pnsHandle) => new DeviceTarget(RuntimePlatform.iOS, pnsHandle);
        public static DeviceTarget UWP(string pnsHandle) => new DeviceTarget(RuntimePlatform.UWP, pnsHandle);
        public static DeviceTarget Android(string pnsHandle) => new DeviceTarget(RuntimePlatform.Android, pnsHandle);
    }
}
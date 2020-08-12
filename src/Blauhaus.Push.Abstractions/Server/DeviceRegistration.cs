using System.Collections.Generic;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions.Common.Templates._Base;

namespace Blauhaus.Push.Abstractions.Server
{
    public class DeviceRegistration : IDeviceRegistration
    {
        public string PushNotificationServiceHandle { get; set; }
        public string? DeviceIdentifier { get; set; }
        public IRuntimePlatform Platform { get; set; }
        public string? UserId { get; set; }
        public string? AccountId { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public List<IPushNotificationTemplate> Templates { get; set; } = new List<IPushNotificationTemplate>();

    }
}
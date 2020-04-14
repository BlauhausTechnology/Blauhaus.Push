using System.Collections.Generic;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions;
using Blauhaus.Push.Abstractions.Server;

namespace Blauhaus.Push.Server.Service
{
    public class DeviceRegistration : IDeviceRegistration
    {
        public string PushNotificationServiceHandle { get; set; }
        public string? DeviceIdentifier { get; set; }
        public IRuntimePlatform Platform { get; set; }
        public string? UserId { get; set; }
        public string? AccountId { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public List<INotificationTemplate> Templates { get; set; } = new List<INotificationTemplate>();

    }
}
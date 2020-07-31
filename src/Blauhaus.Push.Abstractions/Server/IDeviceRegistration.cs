using System.Collections.Generic;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions.Common.Templates._Base;

namespace Blauhaus.Push.Abstractions.Server
{
    public interface IDeviceRegistration
    {
        string? AccountId { get; set; }
        string UserId { get; set; }
        IRuntimePlatform Platform { get; set; }
        string DeviceIdentifier { get; set; }
        string PushNotificationServiceHandle { get; set; }
        List<string> Tags { get; set; }
        List<IPushNotificationTemplate> Templates { get; set; }
    }
}
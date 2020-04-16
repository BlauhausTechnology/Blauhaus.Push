using System.Collections.Generic;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions.Common.PushNotificationTemplates;
using Blauhaus.Push.Abstractions.Common.PushNotificationTemplates._Base;

namespace Blauhaus.Push.Abstractions.Server
{
    public interface IDeviceRegistration
    {
        string PushNotificationServiceHandle { get; set; }
        string? DeviceIdentifier { get; set; }
        IRuntimePlatform Platform { get; set; }
        string? UserId { get; set; }
        string? AccountId { get; set; }
        List<string> Tags { get; set; }
        List<INotificationTemplate> Templates { get; set; }
    }
}
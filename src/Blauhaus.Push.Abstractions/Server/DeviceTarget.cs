using Blauhaus.Common.ValueObjects.RuntimePlatforms;

namespace Blauhaus.Push.Abstractions.Server
{
    public interface IDeviceTarget
    {
        IRuntimePlatform Platform { get; }
        string PushNotificationServicesHandle { get; }
         
    }
}
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Responses;

namespace Blauhaus.Push.Server.Extractors
{
    public interface INativeNotificationExtractor
    {
        Response<NativeNotification> ExtractNotification(IRuntimePlatform platform, IPushNotification pushNotification);
        Response<NativeNotification> ExtractIosNotification(IPushNotification pushNotification);
        Response<NativeNotification> ExtractUwpNotification(IPushNotification pushNotification);
        Response<NativeNotification> ExtractAndroidNotification(IPushNotification pushNotification);
    }
}
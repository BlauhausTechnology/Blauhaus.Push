using System;
using System.Collections.Generic;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions.Common.Notifications;
using CSharpFunctionalExtensions;
using Microsoft.Azure.NotificationHubs;

namespace Blauhaus.Push.Server.Extractors
{
    public interface INativeNotificationExtractor
    {
        Result<NativeNotification> ExtractNotification(IRuntimePlatform platform, IPushNotification pushNotification);
        Result<NativeNotification> ExtractIosNotification(IPushNotification pushNotification);
        Result<NativeNotification> ExtractUwpNotification(IPushNotification pushNotification);
        Result<NativeNotification> ExtractAndroidNotification(IPushNotification pushNotification);
    }
}
using System;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Microsoft.Azure.NotificationHubs;

namespace Blauhaus.Push.Server.Extensions
{
    public static class RuntimePlatformExtensions
    {
        public static NotificationPlatform ToNotificationPlatform(this IRuntimePlatform runtimePlatform)
        {
            if (runtimePlatform.Equals(RuntimePlatform.iOS)) return NotificationPlatform.Apns;
            if (runtimePlatform.Equals(RuntimePlatform.Android)) return NotificationPlatform.Fcm;
            if (runtimePlatform.Equals(RuntimePlatform.UWP)) return  NotificationPlatform.Wns;

            throw new ArgumentException($"{runtimePlatform} is not currently available as a push notification platform");
        }
    }
}
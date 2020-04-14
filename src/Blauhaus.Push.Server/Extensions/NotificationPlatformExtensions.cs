using System;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Microsoft.Azure.NotificationHubs;

namespace Blauhaus.Push.Server.Extensions
{
    public static class NotificationPlatformExtensions
    {
        public static IRuntimePlatform ToRuntimePlatform(this NotificationPlatform notificationPlatform)
        {
            switch (notificationPlatform)
            {
                case NotificationPlatform.Wns:
                    return RuntimePlatform.UWP;
                case NotificationPlatform.Apns:
                    return RuntimePlatform.iOS;
                case NotificationPlatform.Fcm:
                    return RuntimePlatform.Android;
                default:
                    throw new ArgumentException($"{notificationPlatform.ToString()} is not currently supported");
            }
        }
    }
}
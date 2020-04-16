using System.Collections.Generic;

namespace Blauhaus.Push.Abstractions.Common.PushNotifications
{
    public interface IPushNotification
    {
        Dictionary<string, object> DataProperties { get; }

        string NotificationType { get; }
        string Title { get; }
        string Body { get; }
    }
}
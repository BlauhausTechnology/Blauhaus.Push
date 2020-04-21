using System.Collections.Generic;
using Microsoft.Azure.NotificationHubs;

namespace Blauhaus.Push.Server.Extractors
{
    public class NativeNotification
    {
        public NativeNotification(Notification notification, Dictionary<string, string> headers = null)
        {
            if(headers == null) headers = new Dictionary<string, string>();

            Notification = notification;
            Headers = headers;
        }

        public Notification Notification { get; }

        public Dictionary<string, string> Headers { get; }
    }
}
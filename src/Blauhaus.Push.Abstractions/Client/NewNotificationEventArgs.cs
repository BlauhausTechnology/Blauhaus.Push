using System;
using Blauhaus.Push.Abstractions.Common.Notifications;

namespace Blauhaus.Push.Abstractions.Client
{
    public class NewNotificationEventArgs : EventArgs
    {
        public NewNotificationEventArgs(IPushNotification newNotification)
        {
            NewNotification = newNotification;
        }

        public IPushNotification NewNotification { get; }
    }

}
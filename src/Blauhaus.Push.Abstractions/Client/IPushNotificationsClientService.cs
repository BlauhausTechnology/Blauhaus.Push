using System;
using System.Threading.Tasks;
using Blauhaus.Push.Abstractions.Common;
using Blauhaus.Push.Abstractions.Common.Notifications;

namespace Blauhaus.Push.Abstractions.Client
{
    public interface IPushNotificationsClientService
    {
        IObservable<IPushNotification> ObserveForegroundNotifications();

        ValueTask<string> GetPushNotificationServiceHandleAsync();


    }
}
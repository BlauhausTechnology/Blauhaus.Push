using System;
using System.Threading.Tasks;
using Blauhaus.Push.Abstractions.Common;

namespace Blauhaus.Push.Abstractions.Client
{
    public interface IPushNotificationsClientService
    {
        IObservable<IPushNotification> ObserveForegroundNotifications();

        ValueTask<string> GetPushNotificationServiceHandleAsync();


    }
}
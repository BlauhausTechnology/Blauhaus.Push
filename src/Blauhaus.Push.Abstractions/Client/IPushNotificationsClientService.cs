using System;
using System.Threading.Tasks;

namespace Blauhaus.Push.Abstractions.Client
{
    public interface IPushNotificationsClientService
    {
        IObservable<IPushNotification> ObserveForegroundNotifications();

        ValueTask<string> GetPushNotificationServiceHandleAsync();


    }
}
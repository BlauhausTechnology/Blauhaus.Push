using System;
using System.Threading.Tasks;
using Blauhaus.Push.Abstractions.Common.PushNotifications;

namespace Blauhaus.Push.Abstractions.Client
{
    public interface IPushNotificationsClientService
    {
        IObservable<IPushNotification> ObserveForegroundNotifications();

        ValueTask<string> GetPushNotificationServiceHandleAsync();


    }
}
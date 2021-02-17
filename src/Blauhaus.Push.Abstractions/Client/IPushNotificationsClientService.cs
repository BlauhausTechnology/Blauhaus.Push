using System;
using System.Threading.Tasks;
using Blauhaus.Common.Utils.Contracts;
using Blauhaus.Push.Abstractions.Common.Notifications;

namespace Blauhaus.Push.Abstractions.Client
{
    public interface IPushNotificationsClientService : IAsyncPublisher<IPushNotification>
    {
        IObservable<IPushNotification> ObserveForegroundNotifications();

        //maybe temporary
        event EventHandler<NewNotificationEventArgs> NewNotificationEvent;

        ValueTask<string> GetPushNotificationServiceHandleAsync();


    }
}
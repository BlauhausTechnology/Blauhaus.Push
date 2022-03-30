using System;
using System.Threading.Tasks;
using Blauhaus.Common.Abstractions;
using Blauhaus.Push.Abstractions.Common.Notifications;

namespace Blauhaus.Push.Abstractions.Client
{
    public interface IPushNotificationsClientService : IAsyncPublisher<IPushNotification>
    {
        //maybe temporary
        event EventHandler<NewNotificationEventArgs> NewNotificationEvent;

        ValueTask<string> GetPushNotificationServiceHandleAsync();


    }
}
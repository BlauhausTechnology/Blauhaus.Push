using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Blauhaus.Common.TestHelpers.MockBuilders;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;

namespace Blauhaus.Push.TestHelpers.MockBuilders
{
    public class PushNotificationsClientServiceMockBuilder : BaseAsyncPublisherMockBuilder<PushNotificationsClientServiceMockBuilder, IPushNotificationsClientService, IPushNotification>
    {
        public PushNotificationsClientServiceMockBuilder()
        {
            Where_GetPushNotificationServiceHandleAsync_returns(Guid.NewGuid().ToString());
            Where_ObserveForegroundNotifications_returns(new PushNotificationMockBuilder().Object);
        }

        public PushNotificationsClientServiceMockBuilder Where_GetPushNotificationServiceHandleAsync_returns(string value)
        {
            Mock.Setup(x => x.GetPushNotificationServiceHandleAsync())
                .ReturnsAsync(value);
            return this;
        }
        
        public PushNotificationsClientServiceMockBuilder Where_ObserveForegroundNotifications_returns(IDisposable token)
        {
            Mock.Setup(x => x.ObserveForegroundNotifications())
                .Returns(Observable.Create<IPushNotification>(observer => token));

            return this;
        }

        public PushNotificationsClientServiceMockBuilder Where_ObserveForegroundNotifications_returns(List<IPushNotification> notifications)
        {
            Mock.Setup(x => x.ObserveForegroundNotifications())
                .Returns(Observable.Create<IPushNotification>(observer =>
                {
                    foreach (var pushNotification in notifications)
                    {
                        observer.OnNext(pushNotification);
                    }
                    return Disposable.Empty;
                }));

            return this;
        }

        public PushNotificationsClientServiceMockBuilder Where_ObserveForegroundNotifications_returns(IPushNotification notification)
        {
            Mock.Setup(x => x.ObserveForegroundNotifications())
                .Returns(Observable.Create<IPushNotification>(observer =>
                {
                    observer.OnNext(notification);
                    return Disposable.Empty;
                }));

            return this;
        }
        
        public PushNotificationsClientServiceMockBuilder Where_ObserveForegroundNotifications_fails(Exception exception)
        {
            Mock.Setup(x => x.ObserveForegroundNotifications())
                .Returns(Observable.Create<IPushNotification>(observer =>
                {
                    observer.OnError(exception);
                    return Disposable.Empty;
                }));

            return this;
        }
    }
}
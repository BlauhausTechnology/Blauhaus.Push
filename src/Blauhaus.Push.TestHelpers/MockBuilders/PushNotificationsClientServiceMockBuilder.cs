using System;
using Blauhaus.Common.TestHelpers.MockBuilders;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Moq;

namespace Blauhaus.Push.TestHelpers.MockBuilders
{
    public class PushNotificationsClientServiceMockBuilder : BaseAsyncPublisherMockBuilder<PushNotificationsClientServiceMockBuilder, IPushNotificationsClientService, IPushNotification>
    {
        public PushNotificationsClientServiceMockBuilder()
        {
            Where_GetPushNotificationServiceHandleAsync_returns(Guid.NewGuid().ToString());
        }

        
        public PushNotificationsClientServiceMockBuilder Where_GetPushNotificationServiceHandleAsync_returns(string value)
        {
            Mock.Setup(x => x.GetPushNotificationServiceHandleAsync())
                .ReturnsAsync(value);
            return this;
        }
         
    }
}
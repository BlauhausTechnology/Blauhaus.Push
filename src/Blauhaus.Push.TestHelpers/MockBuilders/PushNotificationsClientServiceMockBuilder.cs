using System;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;

namespace Blauhaus.Push.TestHelpers.MockBuilders
{
    public class PushNotificationsClientServiceMockBuilder : BaseMockBuilder<PushNotificationsClientServiceMockBuilder, IPushNotificationsClientService>
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
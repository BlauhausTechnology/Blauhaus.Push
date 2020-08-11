using System.Threading;
using Blauhaus.Push.Server.HubClientProxy;
using Blauhaus.TestHelpers.MockBuilders;
using Moq;

namespace Blauhaus.Push.Tests.MockBuilders
{
    public class NotificationHubClientProxyMockBuilder : BaseMockBuilder<NotificationHubClientProxyMockBuilder, INotificationHubClientProxy>
    {
        public NotificationHubClientProxyMockBuilder()
        {
            Mock.Setup(x => x.InstallationExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);
        }
    }
}
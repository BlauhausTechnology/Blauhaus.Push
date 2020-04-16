using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Analytics.TestHelpers;
using Blauhaus.Push.Server._Config;
using Blauhaus.Push.Server.HubClientProxy;
using Blauhaus.Push.Tests.Server.MockBuilders;
using Blauhaus.TestHelpers.BaseTests;
using Blauhaus.TestHelpers.MockBuilders;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Server.Tests._Base
{
    public abstract class BasePushNotificationsServerTest<TSut>: BaseServiceTest<TSut> where TSut : class
    {

        [SetUp]
        public virtual void SetUp()
        {
            Cleanup();
            Services.AddSingleton<TSut>();
            Services.AddSingleton(x => MockPushNotificationsServerConfig.Object);
            Services.AddSingleton(x => MockNotificationHubClientProxy.Object);
            Services.AddSingleton(x => MockAnalyticsService.Object);
        }

        protected MockBuilder<IPushNotificationsServerConfig> MockPushNotificationsServerConfig => Mocks.AddMock<IPushNotificationsServerConfig>().Invoke();
        protected NotificationHubClientProxyMockBuilder MockNotificationHubClientProxy => Mocks.AddMock<NotificationHubClientProxyMockBuilder, INotificationHubClientProxy>().Invoke();
        protected AnalyticsServiceMockBuilder MockAnalyticsService => Mocks.AddMock<AnalyticsServiceMockBuilder, IAnalyticsService>().Invoke();
    }
}
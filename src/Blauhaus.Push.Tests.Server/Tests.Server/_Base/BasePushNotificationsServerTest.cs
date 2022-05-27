using Blauhaus.Analytics.Abstractions;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Analytics.TestHelpers;
using Blauhaus.Analytics.TestHelpers.MockBuilders;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Server.Extractors;
using Blauhaus.Push.Server.HubClientProxy;
using Blauhaus.Push.Tests.MockBuilders;
using Blauhaus.TestHelpers.BaseTests;
using Blauhaus.TestHelpers.MockBuilders;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Tests.Server._Base
{
    public abstract class BasePushNotificationsServerTest<TSut>: BaseServiceTest<TSut> where TSut : class
    {

        [SetUp]
        public virtual void SetUp()
        {
            Cleanup();
            Services.AddSingleton<TSut>();
            Services.AddSingleton(x => MockNotificationHub.Object);
            Services.AddSingleton(x => MockNotificationHubClientProxy.Object);
            Services.AddSingleton(x => MockLogger.Object);
            Services.AddSingleton(x => MockNativeNotificationExtractor.Object);
        }

        protected MockBuilder<IPushNotificationsHub> MockNotificationHub => Mocks.AddMock<IPushNotificationsHub>().Invoke();
        protected NotificationHubClientProxyMockBuilder MockNotificationHubClientProxy => Mocks.AddMock<NotificationHubClientProxyMockBuilder, INotificationHubClientProxy>().Invoke();
        protected AnalyticsLoggerMockBuilder<TSut> MockLogger => Mocks.AddMock<AnalyticsLoggerMockBuilder<TSut>, IAnalyticsLogger<TSut>>().Invoke();
        protected NativeNotificationExtractorMockBuilder MockNativeNotificationExtractor => Mocks.AddMock<NativeNotificationExtractorMockBuilder, INativeNotificationExtractor>().Invoke();
    }
}
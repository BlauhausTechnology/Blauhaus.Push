using Blauhaus.Analytics.Abstractions;
using Blauhaus.DeviceServices.Abstractions.SecureStorage;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Client.Common.Base;

namespace Blauhaus.Push.Tests.Tests.Client.PushNotificationsClientServiceTests.BasePushNotificationsClientServiceTests._Base
{
    public class TestPushNotificationsClientService : BasePushNotificationsClientService
    {
        public TestPushNotificationsClientService(
            ISecureStorageService secureStorageService,
            IAnalyticsLogger<TestPushNotificationsClientService> logger,
            IPushNotificationTapHandler pushNotificationTapHandler) 
            : base(logger, secureStorageService, pushNotificationTapHandler)
        {
        }
    }
}
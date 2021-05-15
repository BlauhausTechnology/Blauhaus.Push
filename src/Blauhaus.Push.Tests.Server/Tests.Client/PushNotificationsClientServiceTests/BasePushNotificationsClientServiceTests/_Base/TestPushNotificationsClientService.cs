using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.DeviceServices.Abstractions.SecureStorage;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Client.Common.Base;

namespace Blauhaus.Push.Tests.Tests.Client.PushNotificationsClientServiceTests.BasePushNotificationsClientServiceTests._Base
{
    public class TestPushNotificationsClientService : BasePushNotificationsClientService
    {
        public TestPushNotificationsClientService(
            ISecureStorageService secureStorageService,
            IAnalyticsService analyticsService,
            IPushNotificationTapHandler pushNotificationTapHandler) 
            : base(analyticsService, secureStorageService, pushNotificationTapHandler)
        {
        }
    }
}
using System.Collections.Generic;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.DeviceServices.Abstractions.SecureStorage;
using Blauhaus.Push.Abstractions.Client;
using Blauhaus.Push.Client.Common._Base;

namespace Blauhaus.Push.Tests.Client.Tests.PushNotificationsClientServiceTests.BasePushNotificationsClientServiceTests._Base
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
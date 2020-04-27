using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions.Common;
using Blauhaus.Push.Abstractions.Common.Notifications;
using Blauhaus.Push.Abstractions.Server;
using Blauhaus.Push.Server.Extractors;
using Blauhaus.Push.Server.Service;
using Blauhaus.Push.TestHelpers.MockBuilders;
using Blauhaus.Push.Tests.Server.Tests._Base;
using Microsoft.Azure.NotificationHubs;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Server.Tests.TargetedNotificationsServerServiceTests
{
    [TestFixture]
    public class SendNotificationToTargetAsyncTests : BasePushNotificationsServerTest<TargetedPushNotificationsServerService>
    {

        private IPushNotification _pushNotification;
        private IDeviceTarget _target;
        private readonly Guid _id = Guid.NewGuid();
        private Notification _iosNotification;

        public override void SetUp()
        {
            base.SetUp();
            _iosNotification = new AppleNotification("{}");
            MockNativeNotificationExtractor.Where_ExtractNotification_returns(new NativeNotification(_iosNotification));

            _pushNotification = new PushNotificationMockBuilder()
                .With(x => x.Name, "TestNotificationType")
                .With(x => x.Title, "News Alert")
                .With(x => x.Body, "The lockdown is only in your mind")
                .With(x => x.DataProperties, new Dictionary<string, object>
                {
                    {"FavouriteColour", "Red"},
                    {"FavouriteBand", "The Beatles"},
                    {"Id", _id },
                }).Object;

            _target = DeviceTarget.Android("pnsHandle");
        }

        [Test]
        public async Task SHOULD_initialize_client()
        {
            //Act
            await Sut.SendNotificationToTargetAsync(_pushNotification, _target, MockNotificationHub.Object, CancellationToken.None);

            //Assert
            MockNotificationHubClientProxy.Mock.Verify(x => x.Initialize(MockNotificationHub.Object));
        }

        [Test]
        public async Task SHOULD_track_operation()
        {
            //Act
            await Sut.SendNotificationToTargetAsync(_pushNotification, _target, MockNotificationHub.Object, CancellationToken.None);

            //Assert
            MockAnalyticsService.VerifyTrace("Send push notification to device");
            MockAnalyticsService.VerifyTraceProperty(nameof(PushNotification), _pushNotification);
            MockAnalyticsService.VerifyTraceProperty(nameof(DeviceTarget), _target);
        }

        [Test]
        public async Task SHOULD_extract_notification_for_target_platform_and_send_to_target_pnsHandle()
        {
            //Arrange
            _target = DeviceTarget.iOS("pnsHandle");

            //Act
            await Sut.SendNotificationToTargetAsync(_pushNotification, _target, MockNotificationHub.Object, CancellationToken.None);

            //Assert
            MockNativeNotificationExtractor.Mock.Verify(x=> x.ExtractNotification(RuntimePlatform.iOS, _pushNotification));
            MockNotificationHubClientProxy.Mock.Verify(x => x.SendDirectNotificationAsync(_iosNotification, It.Is<List<string>>(y => 
                y[0] == "pnsHandle"), It.IsAny<CancellationToken>()));
        }

        [Test]
        public async Task SHOULD_trace_extracted_notification()
        {
            //Arrange
            _target = DeviceTarget.iOS("pnsHandle");

            //Act
            await Sut.SendNotificationToTargetAsync(_pushNotification, _target, MockNotificationHub.Object, CancellationToken.None);

            //Assert
            MockAnalyticsService.VerifyTrace("Native push notification extracted");
            MockAnalyticsService.VerifyTraceProperty(_iosNotification.GetType().Name, _iosNotification);
        }

        [Test]
        public async Task IF_extraction_fails_SHOULD_fail()
        {
            //Arrange
            MockNativeNotificationExtractor.Where_ExtractNotification_fails("fail!");

            //Act
            var result = await Sut.SendNotificationToTargetAsync(_pushNotification, _target, MockNotificationHub.Object, CancellationToken.None);

            //Assert
            Assert.AreEqual("fail!", result.Error);
        } 

        [Test]
        public async Task IF_hub_throws_exception_SHOULD_return_fail()
        {
            //Arrange
            var ex = new Exception("Something bad happened");
            MockNotificationHubClientProxy.Mock.Setup(x => x.SendDirectNotificationAsync(It.IsAny<Notification>(), It.IsAny<List<string>>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(ex);

            //Act
            var result = await Sut.SendNotificationToTargetAsync(_pushNotification, _target, MockNotificationHub.Object, CancellationToken.None);

            //Assert
            Assert.AreEqual(PushErrors.FailedToSendNotification.ToString(), result.Error);
            MockAnalyticsService.VerifyLogException(ex);
        }
    }
}
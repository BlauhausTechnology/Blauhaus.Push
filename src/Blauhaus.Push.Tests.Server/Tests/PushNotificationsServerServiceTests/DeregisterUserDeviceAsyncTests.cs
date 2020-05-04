using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Common.ValueObjects.RuntimePlatforms;
using Blauhaus.Push.Abstractions.Common;
using Blauhaus.Push.Server.Service;
using Blauhaus.Push.Tests.Server.Tests._Base;
using Microsoft.Azure.NotificationHubs;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Server.Tests.PushNotificationsServerServiceTests
{
    [TestFixture]
    public class DeregisterUserDeviceAsyncTests : BasePushNotificationsServerTest<PushNotificationsServerService>
    {
        private Installation _installation;


        public override void SetUp()
        {
            base.SetUp();

            _installation = new Installation
            {
                InstallationId = "myUserId___myDeviceId",
                Platform = NotificationPlatform.Fcm,
                ExpirationTime = null,
                PushChannel = "myPnsHandle",
                PushChannelExpired = false,
                PushVariables = null,
                Tags = new List<string> {"TagOne", "TagTwo"},
                Templates = new Dictionary<string, InstallationTemplate>
                {
                    {
                        "DummyTemplate", new InstallationTemplate
                        {
                            Body = "{ \"data\" : { \"DummyPropertyOne\" : \"$(DummyPropertyOne)\", \"DummyPropertyTwo\" : \"$(DummyPropertyTwo)\" }}"
                        }
                    }
                }
            };
            MockNotificationHubClientProxy.Mock.Setup(x => x.GetInstallationAsync(_installation.InstallationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_installation);
        }

        [Test]
        public async Task SHOULD_track_operation()
        {
            //Act
            await Sut.DeregisterUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken);

            //Assert
            MockAnalyticsService.VerifyTrace("Deregister user device");
            MockAnalyticsService.VerifyTraceProperty("UserId", "myUserId");
            MockAnalyticsService.VerifyTraceProperty("DeviceIdentifier", "myDeviceId");
        }

        [Test]
        public async Task SHOULD_initialize_client()
        {
            //Act
            var result = await Sut.DeregisterUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken);

            //Assert
            MockNotificationHubClientProxy.Mock.Verify(x => x.Initialize(MockNotificationHub.Object));
        }

        [Test]
        public async Task IF_installation_does_not_exist_SHOULD_succeed_and_trace()
        {
            //Arrange
            MockNotificationHubClientProxy.Mock.Setup(x => x.InstallationExistsAsync(_installation.InstallationId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            //Act
            var result = await Sut.DeregisterUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken);

            //Assert
            Assert.IsTrue(result.IsSuccess);
            MockAnalyticsService.VerifyTrace("No installation exists for user device, so there is nothing to deregister", LogSeverity.Warning);
        }

        [Test]
        public async Task SHOULD_clear_templates_for_loaded_installation_and_then_save()
        {
            //Act
            await Sut.DeregisterUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken);

            //Assert
            MockNotificationHubClientProxy.Mock.Verify(x => x.GetInstallationAsync(_installation.InstallationId, CancellationToken));
            MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y => 
                y.InstallationId == _installation.InstallationId &&
                y.Templates.Count == 0), CancellationToken));
        }
        
        [Test]
        public async Task IF_installation_Templates_null_SHOULD_set_to_empty()
        {
            //Arrange
            _installation.Templates = null;

            //Act
            await Sut.DeregisterUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken);

            //Assert
            MockNotificationHubClientProxy.Mock.Verify(x => x.GetInstallationAsync(_installation.InstallationId, CancellationToken));
            MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y => 
                y.InstallationId == _installation.InstallationId &&
                y.Templates.Count == 0), CancellationToken));
        }

        [Test]
        public async Task SHOULD_trace_success()
        {
            //Act
            await Sut.DeregisterUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object, CancellationToken);

            //Assert
            MockAnalyticsService.VerifyTrace("Templates cleared for push notifications registration");
        }


    }
}
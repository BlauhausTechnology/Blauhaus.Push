using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blauhaus.Analytics.Abstractions.Service;
using Blauhaus.Push.Server.Service;
using Blauhaus.Push.Tests.Tests.Server._Base;
using Microsoft.Azure.NotificationHubs;
using Moq;
using NUnit.Framework;

namespace Blauhaus.Push.Tests.Tests.Server.PushNotificationsServerServiceTests
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
            MockNotificationHubClientProxy.Mock.Setup(x => x.GetInstallationAsync(_installation.InstallationId))
                .ReturnsAsync(_installation);
        }
         
        [Test]
        public async Task SHOULD_initialize_client()
        {
            //Act
            var result = await Sut.DeregisterUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object);

            //Assert
            MockNotificationHubClientProxy.Mock.Verify(x => x.Initialize(MockNotificationHub.Object));
        }

        [Test]
        public async Task IF_installation_does_not_exist_SHOULD_succeed_and_trace()
        {
            //Arrange
            MockNotificationHubClientProxy.Mock.Setup(x => x.InstallationExistsAsync(_installation.InstallationId))
                .ReturnsAsync(false);

            //Act
            var result = await Sut.DeregisterUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object);

            //Assert
            Assert.IsTrue(result.IsSuccess); 
        }

        [Test]
        public async Task SHOULD_clear_templates_for_loaded_installation_and_then_save()
        {
            //Act
            await Sut.DeregisterUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object);

            //Assert
            MockNotificationHubClientProxy.Mock.Verify(x => x.GetInstallationAsync(_installation.InstallationId));
            MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y => 
                y.InstallationId == _installation.InstallationId &&
                y.Templates.Count == 0)));
        }
        
        [Test]
        public async Task IF_installation_Templates_null_SHOULD_set_to_empty()
        {
            //Arrange
            _installation.Templates = null;

            //Act
            await Sut.DeregisterUserDeviceAsync("myUserId", "myDeviceId", MockNotificationHub.Object);

            //Assert
            MockNotificationHubClientProxy.Mock.Verify(x => x.GetInstallationAsync(_installation.InstallationId));
            MockNotificationHubClientProxy.Mock.Verify(x => x.CreateOrUpdateInstallationAsync(It.Is<Installation>(y => 
                y.InstallationId == _installation.InstallationId &&
                y.Templates.Count == 0)));
        }
         

    }
}